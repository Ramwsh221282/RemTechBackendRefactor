using Microsoft.Extensions.Options;
using Npgsql;
using RemTech.Shared.Configuration.Options;

namespace Shared.Infrastructure.Module.Postgres;

public sealed class PostgresDatabase
{
    public NpgsqlDataSource DataSource { get; }

    public PostgresDatabase(IOptions<DatabaseOptions> options)
    {
        NpgsqlDataSourceBuilder builder = new NpgsqlDataSourceBuilder(
            options.Value.ToConnectionString()
        );
        builder.UseVector();
        DataSource = builder.Build();
    }
}
