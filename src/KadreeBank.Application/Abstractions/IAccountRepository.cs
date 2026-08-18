using KadreeBank.Domain.Entities;

namespace KadreeBank.Application.Abstractions;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Account account, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Account>> ListAsync(CancellationToken cancellationToken = default);
    Task ExecuteMutationAsync(
        Guid accountId,
        Func<Account, AccountTransaction> mutate,
        CancellationToken cancellationToken = default);
}
