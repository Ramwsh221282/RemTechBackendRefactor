using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Pgvector.Dapper;
using RemTech.Shared.Configuration.Options;

namespace Shared.Infrastructure.Module.Postgres;

public sealed class PostgresDatabase
{
    public NpgsqlDataSource DataSource { get; }

    public PostgresDatabase(IOptions<DatabaseOptions> options)
    {
        SqlMapper.AddTypeHandler(new VectorTypeHandler());
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        NpgsqlDataSourceBuilder builder = new NpgsqlDataSourceBuilder(
            options.Value.ToConnectionString()
        );
        DataSource = builder.Build();
    }
}
