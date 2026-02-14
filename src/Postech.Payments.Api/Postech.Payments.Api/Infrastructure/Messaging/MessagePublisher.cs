using MassTransit;
using Postech.Payments.Api.Domain.Events;

namespace Postech.Payments.Api.Infrastructure.Messaging;

public class MessagePublisher : IMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MessagePublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishPaymentProcessedAsync(PaymentProcessedEvent paymentProcessedEvent)
    {
        await _publishEndpoint.Publish(paymentProcessedEvent);
    }
}

