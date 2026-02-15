using Microsoft.EntityFrameworkCore;
using Postech.Payments.Api.Infrastructure.Repositories;

namespace Postech.Payments.Api.Infrastructure.Data;

public static class EfCoreConfig
{
    public static IServiceCollection AddEfCoreDatabase(this IServiceCollection services, IConfiguration configuration)
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

        return services;
    }
}