using KadreeBank.Domain.Common;
using KadreeBank.Domain.Enums;

namespace KadreeBank.Domain.Entities;

public class Account
{
    private readonly List<AccountTransaction> _transactions = [];

    private Account()
    {
    }

    public static Account Open(
        Customer customer,
        string number,
        string originCity,
        AccountType type,
        decimal initialBalance = 0,
        Guid? id = null)
    {
        ArgumentNullException.ThrowIfNull(customer);

        if (string.IsNullOrWhiteSpace(number))
        {
            throw new DomainException("El número de cuenta es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(originCity))
        {
            throw new DomainException("La ciudad de origen es obligatoria.");
        }

        if (type == AccountType.Savings && customer.Kind != CustomerKind.NaturalPerson)
        {
            throw new DomainException("Las cuentas de ahorros son solo para personas naturales.");
        }

        if (type == AccountType.Checking && customer.Kind != CustomerKind.Company)
        {
            throw new DomainException("Las cuentas corrientes son solo para empresas.");
        }

        if (initialBalance < 0)
        {
            throw new DomainException("Una cuenta no puede tener saldo negativo.");
        }

        var account = new Account
        {
            Id = id ?? Guid.NewGuid(),
            CustomerId = customer.Id,
            Number = number.Trim(),
            OriginCity = originCity.Trim(),
            Type = type,
            Balance = decimal.Round(initialBalance, 2, MidpointRounding.AwayFromZero)
        };

        if (account.Balance > 0)
        {
            account._transactions.Add(new AccountTransaction(
                account.Id,
                TransactionType.Deposit,
                account.Balance,
                account.OriginCity,
                DateTimeOffset.UtcNow));
        }

        return account;
    }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string OriginCity { get; private set; } = string.Empty;
    public AccountType Type { get; private set; }
    public decimal Balance { get; private set; }

    public IReadOnlyCollection<AccountTransaction> Transactions => _transactions;

    public AccountTransaction Deposit(decimal amount, string city, DateTimeOffset occurredAt)
        => Apply(TransactionType.Deposit, amount, city, occurredAt);

    public AccountTransaction Withdraw(decimal amount, string city, DateTimeOffset occurredAt)
    {
        if (amount > Balance)
        {
            throw new DomainException("Una cuenta no puede tener saldo negativo.");
        }

        return Apply(TransactionType.Withdrawal, amount, city, occurredAt);
    }

    private AccountTransaction Apply(TransactionType type, decimal amount, string city, DateTimeOffset occurredAt)
    {
        var movement = new AccountTransaction(Id, type, amount, city, occurredAt);
        Balance = type == TransactionType.Deposit
            ? Balance + movement.Amount
            : Balance - movement.Amount;
        _transactions.Add(movement);
        return movement;
    }
}
