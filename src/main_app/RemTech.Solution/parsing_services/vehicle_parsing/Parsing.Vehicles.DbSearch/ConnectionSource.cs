using Npgsql;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

namespace Parsing.Vehicles.DbSearch;

public sealed class ConnectionSource : IDisposable, IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource;

    public ConnectionSource(DatabaseConfiguration configuration)
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
}