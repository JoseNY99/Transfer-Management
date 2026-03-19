using Microsoft.EntityFrameworkCore;
using Payments.Application.Abstractions.Persistence;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
    }

    public Task<Payment?> GetByExternalOperationIdAsync(Guid externalOperationId, CancellationToken cancellationToken)
    {
        return _context.Payments.FirstOrDefaultAsync(x => x.ExternalOperationId == externalOperationId, cancellationToken);
    }

    public async Task<decimal> GetDailyAccumulatedAmountAsync(Guid customerId, DateTime dayUtc, CancellationToken cancellationToken)
    {
        var start = dayUtc.Date;
        var end = start.AddDays(1);

        return await _context.Payments
            .Where(x => x.CustomerId == customerId && x.CreatedAt >= start && x.CreatedAt < end)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
    }
}
