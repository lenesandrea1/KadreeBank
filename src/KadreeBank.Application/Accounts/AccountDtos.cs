namespace KadreeBank.Application.Accounts;

public record AccountSummaryDto(
    Guid Id,
    string Number,
    string Type,
    string OriginCity,
    decimal Balance,
    Guid CustomerId);

public record BalanceDto(Guid AccountId, string Number, decimal Balance, string OriginCity, string Type);

public record MovementDto(
    Guid Id,
    string Type,
    decimal Amount,
    string City,
    DateTimeOffset OccurredAt);

public record MonthlyStatementDto(
    Guid AccountId,
    string Number,
    int Year,
    int Month,
    decimal OpeningBalance,
    decimal ClosingBalance,
    IReadOnlyList<MovementDto> Movements);

public record MoneyOperationRequest(decimal Amount, string City);
