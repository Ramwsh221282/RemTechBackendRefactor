using Microsoft.Extensions.Options;
using Npgsql;

namespace RemTech.NpgSql.Abstractions;

public sealed class NpgSqlConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgSqlConnectionFactory(IOptions<NpgSqlOptions> options)
    {
        var builder = new NpgsqlDataSourceBuilder(options.Value.ToConnectionString());
        builder.UseVector();
        _dataSource = builder.Build();
    }

    public async Task<NpgsqlConnection> Create(CancellationToken ct = default)
    {
        return await _dataSource.OpenConnectionAsync(ct);
    }
}