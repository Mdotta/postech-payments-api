using Postech.Payments.Api.Domain.Events;

namespace Postech.Payments.Api.Infrastructure.Messaging;

public interface IMessagePublisher
{
    Task PublishPaymentProcessedAsync(PaymentProcessedEvent paymentProcessedEvent);
}

