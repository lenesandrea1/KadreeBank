using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KadreeBank.Infrastructure.Persistence;

public class KadreeBankDbContextFactory : IDesignTimeDbContextFactory<KadreeBankDbContext>
{
    public KadreeBankDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<KadreeBankDbContext>()
            .UseNpgsql("Host=127.0.0.1;Port=55432;Database=kadreebank;Username=kadree;Password=kadree;SSL Mode=Disable")
            .Options;

        return new KadreeBankDbContext(options);
    }
}
