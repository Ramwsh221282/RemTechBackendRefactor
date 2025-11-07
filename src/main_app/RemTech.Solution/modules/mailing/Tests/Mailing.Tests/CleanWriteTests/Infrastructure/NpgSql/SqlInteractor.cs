using System.Data;
using Dapper;
using RemTech.Core.Shared.Async;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;

public sealed class SqlInteractor(Func<Task<IDbConnection>> connectionSource) : IDisposable
{
    private readonly Lazy<Task<IDbConnection>> _connectionSource = new(connectionSource);
    private readonly Queue<Func<IDbConnection, Task>> _commands = [];
    private IDbConnection? _cachedConnection;

    public async Task ExecuteEnqueued()
    {
        while (_commands.Count > 0)
        {
            Func<IDbConnection, Task> command = _commands.Dequeue();
            await command(await GetConnection());
        }
    }

    public async Task ExecutePermanent(Func<IDbConnection, Task> command)
    {
        IDbConnection conn = await GetConnection();
        await command(conn);
    }

    public FromFuture<T> ExecutePermanent<T>(Func<IDbConnection, Task<T>> command) =>
        new FromFuture<T>(async () => await command(await GetConnection()));

    public async Task ExecutePermanent(CommandDefinition command)
    {
        IDbConnection conn = await GetConnection();
        await conn.ExecuteAsync(command);
    }

    public void Enqueue(Func<IDbConnection, Task> command) => _commands.Enqueue(command);
    public void Enqueue(CommandDefinition command) => _commands.Enqueue((conn) => conn.ExecuteAsync(command));

    public void Dispose()
    {
        if (!_connectionSource.IsValueCreated) return;
        _connectionSource.Value.Dispose();
        if (_connectionSource.Value.IsCompletedSuccessfully) _connectionSource.Value.Dispose();
    }

    private async Task<IDbConnection> GetConnection()
    {
        _cachedConnection ??= await _connectionSource.Value;
        return _cachedConnection;
    }
}