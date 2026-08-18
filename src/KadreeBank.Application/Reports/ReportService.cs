using KadreeBank.Application.Abstractions;

namespace KadreeBank.Application.Reports;

public interface IReportService
{
    Task<IReadOnlyList<CustomerTransactionCountDto>> GetCustomersByTransactionCountAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OffCityWithdrawalDto>> GetOffCityWithdrawalsAsync(
        CancellationToken cancellationToken = default);
}

public class ReportService : IReportService
{
    public const decimal OffCityMinimumTotal = 1_000_000m;

    private readonly IReportQuery _query;

    public ReportService(IReportQuery query)
    {
        _query = query;
    }

    public Task<IReadOnlyList<CustomerTransactionCountDto>> GetCustomersByTransactionCountAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month));
        }

        return _query.GetCustomersByTransactionCountAsync(year, month, cancellationToken);
    }

    public Task<IReadOnlyList<OffCityWithdrawalDto>> GetOffCityWithdrawalsAsync(
        CancellationToken cancellationToken = default)
        => _query.GetOffCityWithdrawalsAsync(OffCityMinimumTotal, cancellationToken);
}
