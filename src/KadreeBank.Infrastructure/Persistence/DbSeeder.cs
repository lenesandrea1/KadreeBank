using KadreeBank.Domain.Entities;
using KadreeBank.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KadreeBank.Infrastructure.Persistence;

public static class DbSeeder
{
    public static readonly Guid AnaAccountId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid AcmeAccountId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static async Task SeedAsync(KadreeBankDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Customers.AnyAsync(cancellationToken))
        {
            return;
        }

        var ana = new Customer(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "Ana Pérez",
            "CC-1001",
            CustomerKind.NaturalPerson);

        var acme = new Customer(
            Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            "Acme SAS",
            "NIT-9001",
            CustomerKind.Company);

        var anaAccount = Account.Open(ana, "AH-001", "Bogotá", AccountType.Savings, 0, AnaAccountId);
        var acmeAccount = Account.Open(acme, "CC-100", "Medellín", AccountType.Checking, 0, AcmeAccountId);

        var august = new DateTimeOffset(2026, 8, 3, 10, 0, 0, TimeSpan.Zero);
        anaAccount.Deposit(5_000_000m, "Bogotá", august);
        anaAccount.Withdraw(200_000m, "Bogotá", august.AddDays(2));
        anaAccount.Withdraw(1_250_000m, "Cali", august.AddDays(5));

        acmeAccount.Deposit(20_000_000m, "Medellín", august.AddHours(1));
        acmeAccount.Withdraw(500_000m, "Medellín", august.AddDays(1));
        acmeAccount.Deposit(3_000_000m, "Medellín", august.AddDays(4));
        acmeAccount.Withdraw(1_500_000m, "Cartagena", august.AddDays(6));

        db.Customers.AddRange(ana, acme);
        db.Accounts.AddRange(anaAccount, acmeAccount);
        await db.SaveChangesAsync(cancellationToken);
    }
}
