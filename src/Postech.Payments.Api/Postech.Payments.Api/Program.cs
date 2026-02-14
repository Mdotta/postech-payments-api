using Postech.Payments.Api.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region [Logging Configuration]

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "Users.Api")
    .CreateLogger();

builder.Host.UseSerilog((context, services, options) =>
{
    options
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext();
});

#endregion


builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.Run();
