using Dapper;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using Serilog;

namespace ParserSubscriber.SubscribtionContext;

public sealed class SubscriptionStorage(NpgSqlConnectionFactory connectionFactory, ILogger? logger)
{
    private ILogger? Logger { get; } = logger?.ForContext<SubscriptionStorage>();
    private string SchemaName { get; set; } = string.Empty;

    public async Task<ParserSubscribtion?> GetSubscription(CancellationToken ct = default)
    {
        await using NpgSqlSession session = new(connectionFactory);
        Logger?.Information("Schema name: {Schema}", SchemaName);
        var sql =
            $"""
             SELECT 
                 id as id, 
                 domain as domain, 
                 type as type, 
                 created as created
             FROM {SchemaName}.subscriptions
             """;
        
        CommandDefinition command = new(sql, cancellationToken: ct);
        return await session.QuerySingleUsingReader(command, reader =>
        {
            Guid id = reader.GetGuid(reader.GetOrdinal("id"));
            string domain = reader.GetString(reader.GetOrdinal("domain"));
            string type = reader.GetString(reader.GetOrdinal("type"));
            DateTime created = reader.GetDateTime(reader.GetOrdinal("created"));
            return new ParserSubscribtion(id, domain, type, created);
        });
    }
    
    public async Task InitializeSubscriptionStorage()
    {
        Logger?.Information("Initializing parsers subscription storage.");
        await using NpgSqlSession session = new(connectionFactory);
        NpgSqlTransactionSource transactionSource = new(session);        
        await using ITransactionScope transaction = await transactionSource.BeginTransaction();

        try
        {
            Logger?.Information("Schema name: {Schema}", SchemaName);
            string schemaSql =
                $"""
                 CREATE SCHEMA IF NOT EXISTS {SchemaName}
                 """;
            string tableSql =
                $"""
                 CREATE TABLE IF NOT EXISTS {SchemaName}.subscriptions
                 (
                     id uuid primary key,
                     domain varchar(128) not null,
                     type varchar(128) not null,
                     created timestamptz not null
                 )
                 """;
        
            await session.Execute(new CommandDefinition(schemaSql, transaction: session.Transaction));
            await session.Execute(new CommandDefinition(tableSql, transaction: session.Transaction));
            Result result = await transaction.Commit();
            if (!result.IsSuccess)
            {
                Logger?.Error(result.Error, "Failed to initialize parsers subscription storage.");
                throw new Exception(result.Error);
            }
        
            Logger?.Information("Initialized parsers subscription storage.");
        }
        catch(Exception e)
        {
            Logger?.Fatal(e, "Failed to initialize parsers subscription storage.");
        }
    }

    public async Task SaveSubscription(ParserSubscribtion subscribtion)
    {
        await InitializeSubscriptionStorage();
        Logger?.Information("Saving parser subscription.");
        Logger?.Information("Schema name: {Schema}", SchemaName);
        object parameters = new
        {
            id = subscribtion.Id,
            domain = subscribtion.Domain,
            type = subscribtion.Type,
            created = subscribtion.Subscribed
        };
        await using NpgSqlSession session = new(connectionFactory);
        NpgSqlTransactionSource transactionSource = new(session);
        await using ITransactionScope transaction = await transactionSource.BeginTransaction();

        try
        {
            var sql =
                $"""
                 INSERT INTO {SchemaName}.subscriptions (id, domain, type, created)
                 VALUES (@id, @domain, @type, @created) ON CONFLICT (id) DO NOTHING
                 """;
            await session.Execute(new CommandDefinition(sql, parameters, transaction: session.Transaction));
        
            Result commit = await transaction.Commit();
            if (!commit.IsSuccess)
            {
                Logger?.Error(commit.Error, "Failed to save parser subscription.");
                throw new Exception(commit.Error);
            }
            Logger?.Information(
                """ 
                Saved parser subscription.
                Id: {Id}
                Domain: {Domain}
                Type: {Type}
                """, subscribtion.Id, subscribtion.Domain, subscribtion.Type);
        }
        catch(Exception e)
        {
            Logger?.Fatal(e, "Failed to save parser subscription.");
        }
    }

    internal void SetSchema(string schema)
    {
        Logger?.Information("Setting schema name: {Schema}", schema);
        SchemaName = schema;
        Logger?.Information("Schema name: {Schema}", SchemaName);
    }
}