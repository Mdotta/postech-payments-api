using Postech.Payments.Api.Domain.Events;

namespace Postech.Payments.Api.Infrastructure.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(T message,CancellationToken cancellationToken = default) where T : class;
}
