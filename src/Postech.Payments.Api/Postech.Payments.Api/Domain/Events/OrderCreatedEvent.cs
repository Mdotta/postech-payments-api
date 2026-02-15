namespace Postech.Payments.Api.Domain.Events;

public class OrderCreatedEvent
{
    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public decimal Price { get; set; }
}