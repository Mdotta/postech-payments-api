using MassTransit;
using Postech.Payments.Api.Application.DTOs;
using Postech.Payments.Api.Application.Services;
using Postech.Payments.Api.Domain.Events;

namespace Postech.Payments.Api.Application.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IPaymentService _paymentService;
    private readonly Serilog.ILogger _logger;

    public OrderCreatedConsumer(IPaymentService paymentService)
    {
        _paymentService = paymentService;
        _logger = Serilog.Log.ForContext<OrderCreatedConsumer>();
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        
        // Configurar contexto de logging com correlationId
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", message.CorrelationId))
        using (Serilog.Context.LogContext.PushProperty("OrderId", message.OrderId))
        using (Serilog.Context.LogContext.PushProperty("UserId", message.UserId))
        using (Serilog.Context.LogContext.PushProperty("GameId", message.GameId))
        {
            try
            {
                _logger.Information(
                    "Consumindo OrderCreatedEvent | OrderId: {OrderId} | UserId: {UserId} | GameId: {GameId} | Valor: R${Price}",
                    message.OrderId,
                    message.UserId,
                    message.GameId,
                    message.Price);

                var dto = new OrderCreatedDto(
                    message.OrderId,
                    message.UserId,
                    message.GameId,
                    message.Price);

                await _paymentService.ProccessOrderAsync(dto);

                _logger.Information(
                    "OrderCreatedEvent processado com sucesso | OrderId: {OrderId}",
                    message.OrderId);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Erro ao processar OrderCreatedEvent | OrderId: {OrderId}",
                    message.OrderId);
                throw;
            }
        }
    }
}

