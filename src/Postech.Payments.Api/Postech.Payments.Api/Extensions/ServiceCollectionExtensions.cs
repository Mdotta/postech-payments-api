using Microsoft.EntityFrameworkCore;
using Postech.Payments.Api.Application.Services;
using Postech.Payments.Api.Infrastructure.Data;
using Postech.Payments.Api.Infrastructure.Messaging;
using Postech.Payments.Api.Infrastructure.Repositories;

namespace Postech.Payments.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, PaymentService>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
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

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IMessagePublisher, MessagePublisher>();

        return services;
    }
}