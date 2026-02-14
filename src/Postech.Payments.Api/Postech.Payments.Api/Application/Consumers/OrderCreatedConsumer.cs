using MassTransit;
using Postech.Payments.Api.Application.DTOs;
using Postech.Payments.Api.Application.Services;
using Postech.Payments.Api.Domain.Events;

namespace Postech.Payments.Api.Application.Consumers;

public class OrderCreatedConsumer :IConsumer<OrderCreatedEvent>
{
    private readonly IPaymentService _paymentService;

    public OrderCreatedConsumer(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        await _paymentService.ProccessOrderAsync(new OrderCreatedDto(
            context.Message.OrderId,
            context.Message.UserId,
            context.Message.GameId,
            context.Message.Price
        ));
    }
}
