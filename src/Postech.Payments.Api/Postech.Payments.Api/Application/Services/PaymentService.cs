using Postech.Payments.Api.Application.DTOs;
using Postech.Payments.Api.Domain.Entities;
using Postech.Payments.Api.Domain.Enums;
using Postech.Payments.Api.Domain.Events;
using Postech.Payments.Api.Infrastructure.Messaging;
using Postech.Payments.Api.Infrastructure.Repositories;

namespace Postech.Payments.Api.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly Random _random;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IMessagePublisher messagePublisher)
    {
        _paymentRepository = paymentRepository;
        _messagePublisher = messagePublisher;
        _random = new Random();
    }

    public async Task ProccessOrderAsync(OrderCreatedDto orderCreatedDto)
    {
        // randomiza sucesso de pagamento (70% chance de sucesso)
        var successChance = _random.Next(0, 100);
        var isSuccessful = successChance < 70;

        var payment = new Payment
        {
            OrderId = orderCreatedDto.OrderId,
            UserId = orderCreatedDto.UserId,
            GameId = orderCreatedDto.GameId,
            Price = orderCreatedDto.Price,
            Status = isSuccessful ? PaymentStatus.Completed : PaymentStatus.Failed,
            CreatedAt = DateTime.UtcNow,
            ProcessedAt = DateTime.UtcNow,
            PaymentMethod = "Pix",
            Message = isSuccessful ? "Payment processed successfully" : "Payment processing failed"
        };

        // salva pagamento no banco
        await _paymentRepository.AddAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        // publica evento de pagamento processado
        var paymentProcessedEvent = new PaymentProcessedEvent
        {
            OrderId = payment.OrderId,
            UserId = payment.UserId,
            GameId = payment.GameId,
            Status = payment.Status
        };

        await _messagePublisher.PublishPaymentProcessedAsync(paymentProcessedEvent);
    }
}
