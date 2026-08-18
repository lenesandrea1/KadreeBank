using KadreeBank.Domain.Common;
using KadreeBank.Domain.Enums;

namespace KadreeBank.Domain.Entities;

public class AccountTransaction
{
    private AccountTransaction()
    {
    }

    internal AccountTransaction(
        Guid accountId,
        TransactionType type,
        decimal amount,
        string city,
        DateTimeOffset occurredAt)
    {
        if (amount <= 0)
        {
            throw new DomainException("El monto debe ser mayor que cero.");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new DomainException("La ciudad de la operación es obligatoria.");
        }

        Id = Guid.NewGuid();
        AccountId = accountId;
        Type = type;
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        City = city.Trim();
        OccurredAt = occurredAt;
    }

    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string City { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
}
