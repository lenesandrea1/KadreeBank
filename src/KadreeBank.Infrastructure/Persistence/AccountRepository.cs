using System.Data;
using KadreeBank.Application.Abstractions;
using KadreeBank.Application.Exceptions;
using KadreeBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace KadreeBank.Infrastructure.Persistence;

public class AccountRepository : IAccountRepository
{
    private readonly KadreeBankDbContext _db;

    public AccountRepository(KadreeBankDbContext db)
    {
        _db = db;
    }

    public Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Accounts
            .AsNoTracking()
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Account>> ListAsync(CancellationToken cancellationToken = default)
        => await _db.Accounts.AsNoTracking().OrderBy(a => a.Number).ToListAsync(cancellationToken);

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await _db.Accounts.AddAsync(account, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ExecuteMutationAsync(
        Guid accountId,
        Func<Account, AccountTransaction> mutate,
        CancellationToken cancellationToken = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            await _db.Database.ExecuteSqlAsync(
                $"""SELECT "Id" FROM accounts WHERE "Id" = {accountId} FOR UPDATE""",
                cancellationToken);

            var account = await _db.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken)
                ?? throw new NotFoundException($"No existe la cuenta {accountId}.");

            var movement = mutate(account);
            _db.Transactions.Add(movement);

            foreach (var entry in _db.ChangeTracker.Entries<AccountTransaction>())
            {
                if (entry.Entity.Id != movement.Id && entry.State == EntityState.Modified)
                {
                    entry.State = EntityState.Unchanged;
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch (Exception ex) when (IsConcurrencyConflict(ex))
        {
            await tx.RollbackAsync(cancellationToken);
            throw new ConcurrencyConflictException();
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static bool IsConcurrencyConflict(Exception ex)
    {
        for (var current = ex; current is not null; current = current.InnerException)
        {
            if (current is DbUpdateConcurrencyException)
            {
                return true;
            }

            if (current is PostgresException pg && pg.SqlState is "40001" or "40P01" or "55P03")
            {
                return true;
            }
        }

        return false;
    }
}
