using System.Data;
using Dapper;
using RemTech.NpgSql.Abstractions;

namespace RemTech.Outbox.Shared;

public sealed class OutboxService
{
    private readonly string _dbSchema;
    private readonly NpgSqlSession _session;
    
    public async Task Add(OutboxMessage message, CancellationToken ct)
    {
        string sql = 
            $"""
             INSERT INTO {_dbSchema}.outbox
             (id, type, body, queue, exchange, routing_key, created_at, processed_at, retry_count)
             VALUES
             (@id, @type, @body::jsonb, @queue, @exchange, @routing_key, @created_at, @processed_at, @retry_count)
             """;
        await _session.Execute(CreateCommand(message, sql, _session, ct));
    }

    public async Task<IEnumerable<OutboxMessage>> GetPendingMessages(int limit, CancellationToken ct)
    {
         string sql = 
             $"""
              SELECT * FROM {_dbSchema}.outbox
              WHERE processed_at IS NULL AND retry_count < 5
              LIMIT @limit 
              """;
         
        CommandDefinition command = new(sql, new { limit }, cancellationToken: ct, transaction: _session.Transaction);
        return await _session.QueryMultipleRows<OutboxMessage>(command);
    }

    public async Task RemoveMany(IEnumerable<OutboxMessage> messages, CancellationToken ct)
    {
        string sql =
            $"""
             DELETE FROM {_dbSchema}.outbox
             WHERE id = ANY(@identifiers)
             """;
        Guid[] identifiers = messages.Select(m => m.Id).ToArray();
        CommandDefinition command = new(sql, new { identifiers }, _session.Transaction, cancellationToken: ct);
        await _session.Execute(command);
    }

    public async Task BeginTransaction(CancellationToken ct)
    {
        await _session.GetTransaction(ct);
    }
    
    public async Task UpdateMany(IEnumerable<OutboxMessage> messages, CancellationToken ct)
    {
        OutboxMessage[] array = messages.ToArray();
        
        string values = string.Join(",\n", array.Select((m, i) => 
            $"(@id{i}, @type{i}, @body{i}::jsonb, @queue{i}, @exchange{i}, @routing_key{i}, @created_at{i}, @processed_at{i}, @retry_count{i})"
        ));

        DynamicParameters parameters = new();
        for (int i = 0; i < array.Length; i++)
        {
            OutboxMessage message = array[i];
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
                      UPDATE {_dbSchema}.outbox
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
                      WHERE {_dbSchema}.outbox.id = dt.id
                      """;
        
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: _session.Transaction);
        await _session.Execute(command);
    }
    
    public async Task<bool> HasAny(CancellationToken ct)
    {
        string sql = $"SELECT COUNT(*) FROM {_dbSchema}.outbox";
        CommandDefinition command = new(sql, null, _session.Transaction, cancellationToken: ct);
        int amount = await _session.QuerySingleRow<int>(command);
        return amount > 0;
    }
    
    private CommandDefinition CreateCommand(OutboxMessage message, string sql, NpgSqlSession session, CancellationToken ct)
    {
        DynamicParameters parameters = FillParameters(message);
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        return command;
    }
    
    private DynamicParameters FillParameters(OutboxMessage message)
    {
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
    
    public OutboxService(NpgSqlSession session, string dbSchema)
    {
        _session = session;
        _dbSchema = dbSchema;
    }
}