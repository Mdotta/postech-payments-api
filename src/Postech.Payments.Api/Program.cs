using Postech.Payments.Api.Extensions;
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

#region [App Extensions]

await app.ApplyMigrationsAsync();
app.ConfigurePipeline();

#endregion

try
{
    Log.Information("Starting Payments API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
