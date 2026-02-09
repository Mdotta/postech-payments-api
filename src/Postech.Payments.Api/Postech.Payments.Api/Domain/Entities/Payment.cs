using System.ComponentModel.DataAnnotations;
using Postech.Payments.Api.Domain.Enums;

namespace Postech.Payments.Api.Domain.Entities;

public class Payment
{
    public int Id { get; set; }

    [Required]
    public Guid OrderId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid GameId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    public decimal Price { get; set; }

    [Required]
    public PaymentStatus Status { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    [MaxLength(50)]
    public string? PaymentMethod { get; set; } = "Pix";

    [MaxLength(500)]
    public string? Message { get; set; }
}