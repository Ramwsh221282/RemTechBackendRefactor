using System.Data;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Pgvector.Dapper;
using RemTech.Core.Shared.Transactions;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using Shared.Infrastructure.Module.Transactions;

namespace Shared.Infrastructure.Module.Postgres;

public static class PostgresDatabaseExtensions
{
    public static async Task<IDbConnection> ProvideConnection(
        this PostgresDatabase database,
        CancellationToken ct = default
    ) => await database.DataSource.OpenConnectionAsync(ct);

    public static IDbTransaction ProvideTransaction(this IDbConnection connection) =>
        connection.BeginTransaction();

    public static void AddPostgres(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(new VectorTypeHandler());
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        services.AddSingleton<PostgresDatabase>();
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddKeyedTransient<IDatabaseUpgrader, PgVectorUpgrader>(nameof(RemTech));
    }

    public static void AddUpgrader<TContext>(this IServiceCollection services, string key)
    {
        services.AddKeyedTransient<IDatabaseUpgrader, DatabaseUpgrader<TContext>>(key);
    }

    public static void InvokeUpgrader(this IServiceProvider provider, string key)
    {
        provider.GetRequiredKeyedService<IDatabaseUpgrader>(key).ApplyMigrations();
    }

    public static void AddEmbeddingGenerator(this IServiceCollection services, string tokenizerPath, string modelPath)
    {
        if (!File.Exists(tokenizerPath))
            throw new InvalidOperationException($"No tokenizer detected at: {tokenizerPath}");

        if (!File.Exists(modelPath))
            throw new InvalidOperationException($"No model detected at: {modelPath}");

        var generator = new OnnxEmbeddingGenerator(tokenizerPath, modelPath);
        services.AddSingleton<IEmbeddingGenerator>(generator);
    }

    public static ITransactionManager ProvideTransactionManager(this PostgresDatabase database) =>
        new TransactionManager(database);
}