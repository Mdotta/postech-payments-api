namespace Postech.Payments.Api.Application.Utils;

public interface ICorrelationContext
{
    Guid CorrelationId { get; set; }
}