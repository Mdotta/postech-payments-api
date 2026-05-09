using Postech.Payments.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Postech.Payments.Api.Infrastructure.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region [Logging Configuration]

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "Payments.Api")
    .CreateLogger();

builder.Host.UseSerilog((context, services, options) =>
{
    options
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext();
});

#endregion


builder.Services.AddHealthChecks();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Apply EF Core migrations at startup with retries and logging
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var db = services.GetRequiredService<PaymentsDbContext>();

        const int maxAttempts = 5;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                logger.LogInformation("Applying database migrations (attempt {Attempt}/{MaxAttempts})", attempt, maxAttempts);
                db.Database.Migrate();
                logger.LogInformation("Database migrations applied successfully.");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying migrations (attempt {Attempt}/{MaxAttempts})", attempt, maxAttempts);
                if (attempt == maxAttempts)
                {
                    throw;
                }

                Thread.Sleep(TimeSpan.FromSeconds(5 * attempt));
            }
        }
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Database migrations failed on startup");
        throw;
    }
}

app.MapHealthChecks("/health");

app.UseHttpsRedirection();

app.Run();
