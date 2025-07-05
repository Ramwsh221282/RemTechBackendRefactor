using RemTech.ParsersManagement.DataSource.Adapter.DataAccessObjects;

namespace RemTech.ParsersManagement.DataSource.Adapter;

public sealed class PostgreSqlParsersSource(PostgreSqlConnectionSource connectionSource)
{
    // private Transaction? _transaction;
    //
    // public async Task<Status<IParser>> Add(IParser parser, CancellationToken ct = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async Task<Status<IParser>> Read(ParserId id, CancellationToken ct = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async Task<Status<IParser>> Read(Name name, CancellationToken ct = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public Task<Status<IParser>> Update(IParser parser, CancellationToken ct = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public Task<Status<IParserLink>> Add(IParserLink parserLink, CancellationToken ct = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public Task<Status<IParserLink>> Remove(IParserLink parserLink, CancellationToken ct = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public Task<Status<IParserLink>> Update(IParserLink parserLink, CancellationToken ct = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async Task<ITransaction> Transactional(CancellationToken ct = default)
    // {
    //     _transaction = await connectionSource.CreateTransaction(ct);
    //     return _transaction;
    // }
    //
    // public async ValueTask DisposeAsync()
    // {
    //     if (_transaction != null)
    //         await _transaction.DisposeAsync();
    // }
    //
    // public void Dispose() => _transaction?.Dispose();
}
