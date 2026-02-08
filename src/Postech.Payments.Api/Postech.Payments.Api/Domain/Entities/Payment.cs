using Postech.Payments.Api.Domain.Enums;

namespace Postech.Payments.Api.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? RejectionReason { get; set; }

    public Payment()
    {
        Id = Guid.NewGuid();
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }
}