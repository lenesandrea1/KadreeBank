using KadreeBank.Application.Abstractions;
using KadreeBank.Application.Exceptions;
using KadreeBank.Domain.Entities;
using KadreeBank.Domain.Enums;

namespace KadreeBank.Application.Accounts;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accounts;

    public AccountService(IAccountRepository accounts)
    {
        _accounts = accounts;
    }

    public async Task<IReadOnlyList<AccountSummaryDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await _accounts.ListAsync(cancellationToken);
        return accounts
            .Select(a => new AccountSummaryDto(a.Id, a.Number, a.Type.ToString(), a.OriginCity, a.Balance, a.CustomerId))
            .ToList();
    }

    public async Task<BalanceDto> GetBalanceAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await GetRequiredAsync(accountId, cancellationToken);
        return MapBalance(account);
    }

    public async Task<IReadOnlyList<MovementDto>> GetRecentMovementsAsync(
        Guid accountId,
        int take = 10,
        CancellationToken cancellationToken = default)
    {
        var account = await GetRequiredAsync(accountId, cancellationToken);
        return account.Transactions
            .OrderByDescending(t => t.OccurredAt)
            .Take(Math.Clamp(take, 1, 50))
            .Select(MapMovement)
            .ToList();
    }

    public async Task<BalanceDto> DepositAsync(
        Guid accountId,
        decimal amount,
        string city,
        CancellationToken cancellationToken = default)
    {
        await _accounts.ExecuteMutationAsync(
            accountId,
            account => account.Deposit(amount, city, DateTimeOffset.UtcNow),
            cancellationToken);
        return await GetBalanceAsync(accountId, cancellationToken);
    }

    public async Task<BalanceDto> WithdrawAsync(
        Guid accountId,
        decimal amount,
        string city,
        CancellationToken cancellationToken = default)
    {
        await _accounts.ExecuteMutationAsync(
            accountId,
            account => account.Withdraw(amount, city, DateTimeOffset.UtcNow),
            cancellationToken);
        return await GetBalanceAsync(accountId, cancellationToken);
    }

    public async Task<MonthlyStatementDto> GetMonthlyStatementAsync(
        Guid accountId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month));
        }

        var account = await GetRequiredAsync(accountId, cancellationToken);
        var start = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        var end = start.AddMonths(1);

        var ordered = account.Transactions.OrderBy(t => t.OccurredAt).ToList();
        var before = ordered.Where(t => t.OccurredAt < start);
        var inMonth = ordered.Where(t => t.OccurredAt >= start && t.OccurredAt < end).ToList();

        var opening = before.Sum(SignedAmount);
        var closing = opening + inMonth.Sum(SignedAmount);

        return new MonthlyStatementDto(
            account.Id,
            account.Number,
            year,
            month,
            opening,
            closing,
            inMonth.Select(MapMovement).ToList());
    }

    private async Task<Account> GetRequiredAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var account = await _accounts.GetByIdAsync(accountId, cancellationToken);
        return account ?? throw new NotFoundException($"No existe la cuenta {accountId}.");
    }

    private static decimal SignedAmount(AccountTransaction t)
        => t.Type == TransactionType.Deposit ? t.Amount : -t.Amount;

    private static BalanceDto MapBalance(Account account)
        => new(account.Id, account.Number, account.Balance, account.OriginCity, account.Type.ToString());

    private static MovementDto MapMovement(AccountTransaction t)
        => new(t.Id, t.Type.ToString(), t.Amount, t.City, t.OccurredAt);
}
