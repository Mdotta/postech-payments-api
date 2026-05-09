using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using Postech.Payments.Api.Application.Services;
using Postech.Payments.Api.Application.Utils;
using Postech.Payments.Api.Infrastructure.Data;
using Postech.Payments.Api.Infrastructure.Messaging;
using Postech.Payments.Api.Infrastructure.Repositories;

namespace Postech.Payments.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICorrelationContext, CorrelationContext>();
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

        var serviceUrl = configuration["AWS:ServiceURL"];

        if (!string.IsNullOrWhiteSpace(serviceUrl))
        {
            services.AddSingleton<IAmazonSimpleNotificationService>(_ =>
                new AmazonSimpleNotificationServiceClient(
                    new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl }));
            services.AddSingleton<IAmazonSQS>(_ =>
                new AmazonSQSClient(
                    new AmazonSQSConfig { ServiceURL = serviceUrl }));
        }
        else
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonSimpleNotificationService>();
            services.AddAWSService<IAmazonSQS>();
        }

        services.AddScoped<IEventPublisher, SnsEventPublisher>();
        services.AddHostedService<SqsOrderPlacedConsumer>();

        return services;
    }
}
