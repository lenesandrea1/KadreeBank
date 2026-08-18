namespace KadreeBank.Application.Accounts;

public interface IAccountService
{
    Task<IReadOnlyList<AccountSummaryDto>> ListAsync(CancellationToken cancellationToken = default);

    Task<BalanceDto> GetBalanceAsync(Guid accountId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MovementDto>> GetRecentMovementsAsync(
        Guid accountId,
        int take = 10,
        CancellationToken cancellationToken = default);

    Task<BalanceDto> DepositAsync(
        Guid accountId,
        decimal amount,
        string city,
        CancellationToken cancellationToken = default);

    Task<BalanceDto> WithdrawAsync(
        Guid accountId,
        decimal amount,
        string city,
        CancellationToken cancellationToken = default);

    Task<MonthlyStatementDto> GetMonthlyStatementAsync(
        Guid accountId,
        int year,
        int month,
        CancellationToken cancellationToken = default);
}
