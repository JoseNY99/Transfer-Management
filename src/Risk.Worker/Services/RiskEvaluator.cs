using Microsoft.EntityFrameworkCore;
using Risk.Worker.Persistence;

namespace Risk.Worker.Services;

public sealed class RiskEvaluator
{
    private readonly RiskReadDbContext _context;

    public RiskEvaluator(RiskReadDbContext context)
    {
        _context = context;
    }

    public async Task<string> EvaluateAsync(Guid customerId, decimal amount, CancellationToken cancellationToken)
    {
        if (amount > 20_000_000m)
            return "denied";

        var todayUtc = DateTime.UtcNow.Date;
        var tomorrowUtc = todayUtc.AddDays(1);

        var accumulated = await _context.Payments
            .Where(x => x.CustomerId == customerId && x.CreatedAt >= todayUtc && x.CreatedAt < tomorrowUtc)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;

        if (accumulated > 5_000_001m)
            return "denied";

        return "accepted";
    }
}
