using System.Data;
using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.Transactions;

public interface ITransactionManager : IDisposable, IAsyncDisposable
{
    Task Begin(CancellationToken ct = default);
    Task<Status> Commit(CancellationToken ct = default);
    void AccessConnection(Action<IDbConnection> action);
    void AccessConnection(Action<IDbConnection, IDbTransaction> action);
    Task Execute(Func<IDbConnection, Task> func);
    Task<T> Execute<T>(Func<IDbConnection, Task<T>> func);
}