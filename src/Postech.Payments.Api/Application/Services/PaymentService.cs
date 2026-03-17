using Postech.Payments.Api.Application.DTOs;
using Postech.Payments.Api.Domain.Entities;
using Postech.Payments.Api.Domain.Enums;
using Postech.Payments.Api.Infrastructure.Messaging;
using Postech.Payments.Api.Infrastructure.Repositories;
using Postech.Shared.Contracts.Events;

namespace Postech.Payments.Api.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly Serilog.ILogger _logger;
    private readonly Random _random;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IEventPublisher eventPublisher)
    {
        _paymentRepository = paymentRepository;
        _eventPublisher = eventPublisher;
        _logger = Serilog.Log.ForContext<PaymentService>();
        _random = new Random();
    }

    public async Task ProccessOrderAsync(OrderCreatedDto orderCreatedDto)
    {
        _logger.Debug(
            "Iniciando processamento de pagamento | OrderId: {OrderId} | Valor: R${Price}",
            orderCreatedDto.OrderId,
            orderCreatedDto.Price);

        try
        {
            // randomiza sucesso de pagamento (70% chance de sucesso)
            var successChance = _random.Next(0, 100);
            var isSuccessful = successChance < 70;

            _logger.Information(
                "Resultado do processamento: {Result} (chance: {Percentage}%)",
                isSuccessful ? "SUCESSO" : "FALHA",
                successChance);

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

            _logger.Debug(
                "Salvando pagamento no banco | OrderId: {OrderId} | Status: {Status}",
                payment.OrderId,
                payment.Status);

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();
            
            using (Serilog.Context.LogContext.PushProperty("PaymentId", payment.Id))
            {
                _logger.Information(
                    "Pagamento salvo com sucesso | PaymentId: {PaymentId} | OrderId: {OrderId} | Status: {Status}",
                    payment.Id,
                    payment.OrderId,
                    payment.Status);

                _logger.Debug(
                    "Preparando para publicar OrderProcessedEvent | OrderId: {OrderId}",
                    payment.OrderId);

                var orderProcessedEvent = new OrderProcessedEvent()
                {
                    OrderId = payment.OrderId,
                    IsSuccessful = isSuccessful,
                    FailureReason = "Failed due to payment processing error"
                };

                await _eventPublisher.PublishAsync(orderProcessedEvent);

                _logger.Information(
                    "OrderProcessedEvent publicado com sucesso | OrderId: {OrderId} | Status: {Status}",
                    payment.OrderId,
                    payment.Status);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(
                ex,
                "Erro ao processar pagamento | OrderId: {OrderId} | Exception: {ExceptionType}",
                orderCreatedDto.OrderId,
                ex.GetType().Name);
            throw;
        }
    }
}

