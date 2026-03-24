using Postech.Payments.Api.Extensions;
using Postech.Payments.Api.Infrastructure.Data;
using Postech.Payments.Api.Infrastructure.MassTransit;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddMassTransitServices(builder.Configuration);
builder.Services.AddEfCoreDatabase(builder.Configuration);
builder.Services.AddApplicationServices();

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

                // simple exponential backoff
                Thread.Sleep(TimeSpan.FromSeconds(5 * attempt));
            }
        }
    }
    catch (Exception ex)
    {
        // If migration fails at startup, log and rethrow to avoid running in a bad state
        Log.Fatal(ex, "Database migrations failed on startup");
        throw;
    }
}

app.MapHealthChecks("/health");

app.UseHttpsRedirection();

app.Run();
