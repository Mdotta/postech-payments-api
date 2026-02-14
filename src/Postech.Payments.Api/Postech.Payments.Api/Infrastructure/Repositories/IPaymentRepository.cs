using Postech.Payments.Api.Domain.Entities;

namespace Postech.Payments.Api.Infrastructure.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);
    Task SaveChangesAsync();
}