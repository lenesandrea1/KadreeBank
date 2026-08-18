using KadreeBank.Application.Reports;

namespace KadreeBank.Application.Abstractions;

public interface IReportQuery
{
    Task<IReadOnlyList<CustomerTransactionCountDto>> GetCustomersByTransactionCountAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OffCityWithdrawalDto>> GetOffCityWithdrawalsAsync(
        decimal minimumTotal,
        CancellationToken cancellationToken = default);
}
