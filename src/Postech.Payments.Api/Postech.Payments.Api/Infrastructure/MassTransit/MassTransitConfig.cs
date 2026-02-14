using MassTransit;
using Postech.Payments.Api.Application.Consumers;

namespace Postech.Payments.Api.Infrastructure.MassTransit;

public static class MassTransitConfig
{
    public static IServiceCollection AddMassTransitServices(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitMqPort = configuration.GetValue<ushort>("RabbitMQ:Port", 5672);
        var rabbitMqUser = configuration["RabbitMQ:Username"] ?? "guest";
        var rabbitMqPass = configuration["RabbitMQ:Password"] ?? "guest";
        var rabbitMqVHost = configuration["RabbitMQ:VirtualHost"] ?? "/";
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqHost,rabbitMqPort,rabbitMqVHost, h =>
                {
                    h.Username(rabbitMqUser);
                    h.Password(rabbitMqPass);
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
        
        
        return services;
    }
}