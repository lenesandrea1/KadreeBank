using KadreeBank.Application.Reports;
using Microsoft.AspNetCore.Mvc;

namespace KadreeBank.Api.Controllers;

[ApiController]
[Route("api/reportes")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;

    public ReportsController(IReportService reports)
    {
        _reports = reports;
    }

    [HttpGet("clientes-transacciones")]
    public async Task<ActionResult<IReadOnlyList<CustomerTransactionCountDto>>> CustomersByTransactions(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
        => Ok(await _reports.GetCustomersByTransactionCountAsync(year, month, cancellationToken));

    [HttpGet("retiros-fuera-ciudad")]
    public async Task<ActionResult<IReadOnlyList<OffCityWithdrawalDto>>> OffCityWithdrawals(
        CancellationToken cancellationToken)
        => Ok(await _reports.GetOffCityWithdrawalsAsync(cancellationToken));
}
