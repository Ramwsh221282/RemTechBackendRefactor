using System.Data;
using Npgsql;
using RemTech.UseCases.Shared.Database;

namespace RemTech.Infrastructure.PostgreSQL;

public sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public DbConnectionFactory(NpgsqlOptions options) =>
        _dataSource = new NpgsqlDataSourceBuilder(options.FormConnectionString()).Build();

    public async Task<IDbConnection> Provide(CancellationToken ct = default) =>
        await _dataSource.OpenConnectionAsync(ct);

    public void Dispose() => _dataSource.Dispose();

    public async ValueTask DisposeAsync() => await _dataSource.DisposeAsync();
}
