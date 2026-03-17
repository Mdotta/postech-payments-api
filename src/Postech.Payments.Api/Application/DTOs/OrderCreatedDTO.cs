namespace Postech.Payments.Api.Application.DTOs;

public class OrderCreatedDto
{
    public OrderCreatedDto(Guid orderId, Guid userId, Guid gameId, decimal price)
    {
        OrderId = orderId;
        UserId = userId;
        GameId = gameId;
        Price = price;
    }

    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public decimal Price { get; set; }
}