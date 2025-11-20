using System.Data;
using System.Text.Json;
using Dapper;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Identity.Outbox.Delegates;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.NpgSql.Abstractions;

namespace Identity.Outbox;

public static class IdentityOutboxMessageModule
{
    public static RemoveMany RemoveMany(NpgSqlSession session) =>
        async (messages, ct) =>
        {
            const string sql =
                """
                DELETE FROM identity_module.outbox
                WHERE id = ANY(@identifiers)
                """;
            Guid[] identifiers = messages.Select(m => m.Id).ToArray();
            CommandDefinition command = new(sql, new { identifiers }, session.Transaction, cancellationToken: ct);
            await session.Execute(command);
        };

    public static UpdateMany UpdateMany(NpgSqlSession session) => async (messages, ct) =>
    {
        IdentityOutboxMessage[] array = messages.ToArray();
        
        string values = string.Join(",\n", array.Select((m, i) => 
            $"(@id{i}, @type{i}, @body{i}::jsonb, @queue{i}, @exchange{i}, @routing_key{i}, @created_at{i}, @processed_at{i}, @retry_count{i})"
        ));

        DynamicParameters parameters = new();
        for (int i = 0; i < array.Length; i++)
        {
            IdentityOutboxMessage message = array[i];
            parameters.Add($"@id{i}", message.Id, DbType.Guid);
            parameters.Add($"@type{i}", message.Type, DbType.String);
            parameters.Add($"@body{i}", message.Body);
            parameters.Add($"@exchange{i}", message.Exchange, DbType.String);
            parameters.Add($"@routing_key{i}", message.RoutingKey, DbType.String);
            parameters.Add($"@created_at{i}", message.CreatedAt, DbType.DateTime);
            parameters.Add($"@queue{i}", message.Queue, DbType.String);
            
            if (message.ProcessedAt.HasValue)
                parameters.Add($"@processed_at{i}", message.ProcessedAt.Value, DbType.DateTime);
            else
                parameters.Add($"@processed_at{i}", null, DbType.DateTime);
            
            parameters.Add($"@retry_count{i}", message.RetryCount, DbType.Int32);
        }
        
        string sql = $"""
                      UPDATE identity_module.outbox
                      SET 
                          type = dt.type,
                          body = dt.body,
                          queue = dt.queue,
                          exchange = dt.exchange,
                          routing_key = dt.routing_key,
                          created_at = dt.created_at,
                          processed_at = dt.processed_at,
                          retry_count = dt.retry_count
                      FROM (VALUES {values}) AS dt(id, type, body, queue, exchange, routing_key, created_at, processed_at, retry_count)
                      WHERE identity_module.outbox.id = dt.id
                      """;

        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        await session.Execute(command);
    };
    
    public static Add Add(NpgSqlSession session) => async (message, ct) =>
    {
        const string sql = 
            """
            INSERT INTO identity_module.outbox
            (id, type, body, queue, exchange, routing_key, created_at, processed_at, retry_count)
            VALUES
            (@id, @type, @body::jsonb, @queue, @exchange, @routing_key, @created_at, @processed_at, @retry_count)
            """;
        await session.Execute(message.CreateCommand(sql, session, ct));
    };

    public static GetPendingMessages GetPendingMessages(NpgSqlSession session) =>
        async (limit, ct) =>
        {
            const string sql = 
                """
                SELECT * FROM identity_module.outbox
                WHERE processed_at IS NULL AND retry_count < 5
                LIMIT @limit 
                """;
            
            CommandDefinition command = new(sql, new { limit }, cancellationToken: ct, transaction: session.Transaction);
            return await session.QueryMultipleRows<IdentityOutboxMessage>(command);
        };
    
    public static HasAny HasAny(NpgSqlSession session) => async (ct) =>
    {
        const string sql = "SELECT COUNT(*) FROM identity_module.outbox";
        int amount = await session.QuerySingleRow<int>(CreateCommand(sql, session, ct));
        return amount > 0;
    };

    private static CommandDefinition CreateCommand(string sql, NpgSqlSession session, CancellationToken ct)
    {
        return new CommandDefinition(sql, cancellationToken: ct, transaction: session.Transaction);
    }

    extension(IdentityOutboxMessage)
    {
        public static IdentityOutboxMessage New(
            string queue, 
            string exchange, 
            string routingKey,
            string type,
            string body)
        {
            return new IdentityOutboxMessage(
                Guid.NewGuid(), 
                type, 
                body, 
                queue,
                exchange, 
                routingKey, 
                DateTime.UtcNow, 
                null,
                0);
        }
    }
        
    extension(IdentityOutboxMessage message)
    {
        public CommandDefinition CreateCommand(string sql, NpgSqlSession session, CancellationToken ct)
        {
            DynamicParameters parameters = message.FillParameters();
            CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
            return command;
        }
            
        public DynamicParameters FillParameters()
        {
            string body = message.Body;
            return NpgSqlParametersStorage
                .New()
                .With("@id", message.Id, DbType.Guid)
                .With("@type", message.Type, DbType.String)
                .With("@body", message.Body, DbType.String)
                .With("@queue", message.Queue, DbType.String)
                .With("@exchange", message.Exchange, DbType.String)
                .With("@routing_key", message.RoutingKey, DbType.String)
                .With("@created_at", message.CreatedAt, DbType.DateTime)
                .WithValueOrNull("@processed_at", message.ProcessedAt, p => p.HasValue, d => d!.Value, DbType.DateTime)
                .With("@retry_count", message.RetryCount, DbType.Int32).GetParameters();  
        }
    }

    extension(IServiceCollection services)
    {
        public void AddIdentityOutbox()
        {
            services.AddScoped<Add>(sp => Add(sp.Resolve<NpgSqlSession>()));
            services.AddScoped<HasAny>(sp => HasAny(sp.Resolve<NpgSqlSession>()));
            services.AddScoped<GetPendingMessages>(sp => GetPendingMessages(sp.Resolve<NpgSqlSession>()));
            services.AddScoped<RemoveMany>(sp => RemoveMany(sp.Resolve<NpgSqlSession>()));
            services.AddScoped<UpdateMany>(sp => UpdateMany(sp.Resolve<NpgSqlSession>()));
            services.AddScoped<IdentityOutboxStorage>();
            services.AddTransient<IDbUpgrader, IdentityOutboxDbUpgrader>();
            services.AddIdentityOutboxProcessor();
        }
    }

    extension(IdentityOutboxStorage)
    {
        public static IdentityOutboxStorage Create(NpgSqlSession session)
        {
            return new IdentityOutboxStorage(
                Add(session), 
                HasAny(session), 
                GetPendingMessages(session),
                RemoveMany(session), 
                UpdateMany(session));
        }
    }
}