using Postech.Payments.Api.Application.DTOs;

namespace Postech.Payments.Api.Application.Services;

public interface IPaymentService
{
    public Task ProccessOrderAsync(OrderCreatedDto orderCreatedDto);
}