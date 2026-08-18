namespace KadreeBank.Application.Reports;

public record CustomerTransactionCountDto(Guid CustomerId, string CustomerName, int TransactionCount);

public record OffCityWithdrawalDto(
    Guid CustomerId,
    string CustomerName,
    string AccountNumber,
    string OriginCity,
    decimal TotalWithdrawn);
