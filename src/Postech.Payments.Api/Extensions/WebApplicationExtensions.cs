using Microsoft.EntityFrameworkCore;
using Postech.Payments.Api.Infrastructure.Data;
using Prometheus;

namespace Postech.Payments.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        app.MapMetrics("/metrics").AllowAnonymous();

        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseHttpMetrics(options => options.AddCustomLabel("service", _ => "payments-api"));

        return app;
    }

    public static async Task<WebApplication> ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<WebApplication>>();

        const int maxAttempts = 5;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                var db = services.GetRequiredService<PaymentsDbContext>();
                logger.LogInformation("Applying database migrations (attempt {Attempt}/{MaxAttempts})", attempt, maxAttempts);
                await db.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
                return app;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying migrations (attempt {Attempt}/{MaxAttempts})", attempt, maxAttempts);
                if (attempt == maxAttempts)
                {
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(5 * attempt));
            }
        }

        return app;
    }
}
