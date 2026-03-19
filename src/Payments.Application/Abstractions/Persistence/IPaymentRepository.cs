using Payments.Domain.Entities;

namespace Payments.Application.Abstractions.Persistence;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken cancellationToken);
    Task<Payment?> GetByExternalOperationIdAsync(Guid externalOperationId, CancellationToken cancellationToken);
    Task<decimal> GetDailyAccumulatedAmountAsync(Guid customerId, DateTime dayUtc, CancellationToken cancellationToken);
}
