using KadreeBank.Application.Abstractions;
using KadreeBank.Application.Reports;
using KadreeBank.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KadreeBank.Infrastructure.Persistence;

public class ReportQuery : IReportQuery
{
    private readonly KadreeBankDbContext _db;

    public ReportQuery(KadreeBankDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CustomerTransactionCountDto>> GetCustomersByTransactionCountAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var start = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        var end = start.AddMonths(1);

        var rows = await (
            from t in _db.Transactions.AsNoTracking()
            join a in _db.Accounts.AsNoTracking() on t.AccountId equals a.Id
            join c in _db.Customers.AsNoTracking() on a.CustomerId equals c.Id
            where t.OccurredAt >= start && t.OccurredAt < end
            select new { c.Id, c.Name }
        ).ToListAsync(cancellationToken);

        return rows
            .GroupBy(x => new { x.Id, x.Name })
            .Select(g => new CustomerTransactionCountDto(g.Key.Id, g.Key.Name, g.Count()))
            .OrderByDescending(x => x.TransactionCount)
            .ToList();
    }

    public async Task<IReadOnlyList<OffCityWithdrawalDto>> GetOffCityWithdrawalsAsync(
        decimal minimumTotal,
        CancellationToken cancellationToken = default)
    {
        var rows = await (
            from t in _db.Transactions.AsNoTracking()
            join a in _db.Accounts.AsNoTracking() on t.AccountId equals a.Id
            join c in _db.Customers.AsNoTracking() on a.CustomerId equals c.Id
            where t.Type == TransactionType.Withdrawal && t.City != a.OriginCity
            select new { c.Id, c.Name, a.Number, a.OriginCity, t.Amount }
        ).ToListAsync(cancellationToken);

        return rows
            .GroupBy(x => new { x.Id, x.Name, x.Number, x.OriginCity })
            .Select(g => new OffCityWithdrawalDto(
                g.Key.Id,
                g.Key.Name,
                g.Key.Number,
                g.Key.OriginCity,
                g.Sum(x => x.Amount)))
            .Where(x => x.TotalWithdrawn > minimumTotal)
            .OrderByDescending(x => x.TotalWithdrawn)
            .ToList();
    }
}
