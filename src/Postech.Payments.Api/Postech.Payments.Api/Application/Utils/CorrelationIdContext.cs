namespace Postech.Payments.Api.Application.Utils;

public class CorrelationContext : ICorrelationContext
{
    public Guid CorrelationId { get; set; }
}