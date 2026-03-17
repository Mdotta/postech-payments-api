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

    
}