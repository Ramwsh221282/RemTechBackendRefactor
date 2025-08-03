using Npgsql;
using Users.Module.Options;

namespace Users.Module.CommonAbstractions;

public sealed class PgConnectionSource(UsersModuleOptions options) : IDisposable, IAsyncDisposable
{
    private readonly NpgsqlDataSource _source = new NpgsqlDataSourceBuilder(
        options.Database.ToConnectionString()
    ).Build();

    public void Dispose() => _source.Dispose();

    public ValueTask DisposeAsync() => _source.DisposeAsync();

    public async Task<NpgsqlConnection> Connect(CancellationToken ct = default) =>
        await _source.OpenConnectionAsync(ct);
}
