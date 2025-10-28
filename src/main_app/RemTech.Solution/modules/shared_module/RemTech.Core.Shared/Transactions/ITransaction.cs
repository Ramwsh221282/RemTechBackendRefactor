using System.Data;
using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.Transactions;

public interface ITransaction : IDisposable, IAsyncDisposable
{
    void Interact(Action<IDbConnection> action);

    void Interact(Action<IDbTransaction?> action);

    Task<T> Interact<T>(Func<IDbConnection, Task<T>> action);

    Task WithTransaction(CancellationToken ct = default);

    public Task<Status> Commit(CancellationToken ct = default);
}
