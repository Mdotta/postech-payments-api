namespace Postech.Payments.Api.Domain.Events;

/// <summary>
/// Evento publicado após o processamento do pagamento
/// </summary>
public record PaymentProcessedEvent
{
    public Guid PaymentId { get; init; }
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public Guid GameId { get; init; }
    public string Status { get; init; } = string.Empty; // "Approved" ou "Rejected"
    public decimal Amount { get; init; }
    public DateTime ProcessedAt { get; init; }
    public string? RejectionReason { get; init; }
}