using Npgsql;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

namespace RemTech.Postgres.Adapter.Library;

public sealed class PostgreSqlEngine : IDisposable, IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgreSqlEngine(DatabaseConfiguration configuration)
    {
        string connectionString = configuration.ConnectionString;
        NpgsqlDataSourceBuilder builder = new(connectionString);
        _dataSource = builder.Build();
    }

    public async Task<NpgsqlConnection> Connect(CancellationToken ct = default) =>
        await _dataSource.OpenConnectionAsync(ct);

    public void Dispose() => _dataSource.Dispose();

    public async ValueTask DisposeAsync() => await _dataSource.DisposeAsync();
}
