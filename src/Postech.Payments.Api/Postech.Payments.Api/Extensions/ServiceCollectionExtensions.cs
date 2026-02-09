using Microsoft.EntityFrameworkCore;
using Postech.Payments.Api.Infrastructure.Data;
using Postech.Payments.Api.Infrastructure.Messaging;

namespace Postech.Payments.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Database connection string is not configured");

        services.AddDbContext<PaymentsDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });
        });

        // Repositories
        // services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Messaging
        services.AddScoped<IEventConsumer, RabbitMqEventConsumer>();

        return services;
    }
    
}