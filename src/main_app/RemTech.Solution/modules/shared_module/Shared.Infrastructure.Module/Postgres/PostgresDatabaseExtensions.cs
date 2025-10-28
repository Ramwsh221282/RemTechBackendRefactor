using System.Data;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Pgvector.Dapper;
using RemTech.Core.Shared.Transactions;
using Shared.Infrastructure.Module.Transactions;

namespace Shared.Infrastructure.Module.Postgres;

public static class PostgresDatabaseExtensions
{
    public static async Task<IDbConnection> ProvideConnection(
        this PostgresDatabase database,
        CancellationToken ct = default
    )
    {
        return await database.DataSource.OpenConnectionAsync(ct);
    }

    public static IDbTransaction ProvideTransaction(this IDbConnection connection) =>
        connection.BeginTransaction();

    public static void AddPostgres(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(new VectorTypeHandler());
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        services.AddSingleton<PostgresDatabase>();
        services.AddScoped<ITransactionManager, TransactionManager>();
    }

    public static ITransactionManager ProvideTransactionManager(this PostgresDatabase database)
    {
        return new TransactionManager(database);
    }
}