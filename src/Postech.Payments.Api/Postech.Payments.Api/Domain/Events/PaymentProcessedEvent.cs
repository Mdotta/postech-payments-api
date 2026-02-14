using Postech.Payments.Api.Domain.Enums;

namespace Postech.Payments.Api.Domain.Events;

public class PaymentProcessedEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public PaymentStatus Status  { get; set; }
}