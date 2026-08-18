using KadreeBank.Application.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace KadreeBank.Api.Controllers;

[ApiController]
[Route("api/cuentas")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accounts;

    public AccountsController(IAccountService accounts)
    {
        _accounts = accounts;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AccountSummaryDto>>> List(CancellationToken cancellationToken)
        => Ok(await _accounts.ListAsync(cancellationToken));

    [HttpGet("{id:guid}/saldo")]
    public async Task<ActionResult<BalanceDto>> GetBalance(Guid id, CancellationToken cancellationToken)
        => Ok(await _accounts.GetBalanceAsync(id, cancellationToken));

    [HttpGet("{id:guid}/movimientos")]
    public async Task<ActionResult<IReadOnlyList<MovementDto>>> GetMovements(
        Guid id,
        [FromQuery] int take = 10,
        CancellationToken cancellationToken = default)
        => Ok(await _accounts.GetRecentMovementsAsync(id, take, cancellationToken));

    [HttpGet("{id:guid}/extractos/{year:int}/{month:int}")]
    public async Task<ActionResult<MonthlyStatementDto>> GetStatement(
        Guid id,
        int year,
        int month,
        CancellationToken cancellationToken)
        => Ok(await _accounts.GetMonthlyStatementAsync(id, year, month, cancellationToken));

    [HttpPost("{id:guid}/consignaciones")]
    public async Task<ActionResult<BalanceDto>> Deposit(
        Guid id,
        [FromBody] MoneyOperationRequest request,
        CancellationToken cancellationToken)
        => Ok(await _accounts.DepositAsync(id, request.Amount, request.City, cancellationToken));

    [HttpPost("{id:guid}/retiros")]
    public async Task<ActionResult<BalanceDto>> Withdraw(
        Guid id,
        [FromBody] MoneyOperationRequest request,
        CancellationToken cancellationToken)
        => Ok(await _accounts.WithdrawAsync(id, request.Amount, request.City, cancellationToken));
}
