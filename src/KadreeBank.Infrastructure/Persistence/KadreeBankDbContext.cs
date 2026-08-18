using KadreeBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KadreeBank.Infrastructure.Persistence;

public class KadreeBankDbContext : DbContext
{
    public KadreeBankDbContext(DbContextOptions<KadreeBankDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountTransaction> Transactions => Set<AccountTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KadreeBankDbContext).Assembly);
    }
}
