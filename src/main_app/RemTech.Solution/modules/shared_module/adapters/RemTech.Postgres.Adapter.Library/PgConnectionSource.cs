using Npgsql;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

namespace RemTech.Postgres.Adapter.Library;

public sealed class PgConnectionSource : IDisposable, IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource;

    public PgConnectionSource(DatabaseConfiguration configuration)
    {
        _dataSource = new NpgsqlDataSourceBuilder(configuration.ConnectionString).Build();
    }
    
    public void Dispose()
    {
        _dataSource.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _dataSource.DisposeAsync();
    }

    public async Task<NpgsqlConnection> Connect()
    {
        return await _dataSource.OpenConnectionAsync();
    }

    public async Task<NpgsqlConnection> Connect(CancellationToken ct)
    {
        return await _dataSource.OpenConnectionAsync(ct);
    }
}