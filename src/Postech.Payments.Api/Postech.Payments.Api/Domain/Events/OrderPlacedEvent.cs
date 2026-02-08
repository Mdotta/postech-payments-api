namespace Postech.Payments.Api.Domain.Events;

/// <summary>
/// Evento recebido quando um pedido é criado no CatalogAPI
/// </summary>
public record OrderPlacedEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public Guid GameId { get; init; }
    public decimal Price { get; init; }
    public DateTime PlacedAt { get; init; }
}