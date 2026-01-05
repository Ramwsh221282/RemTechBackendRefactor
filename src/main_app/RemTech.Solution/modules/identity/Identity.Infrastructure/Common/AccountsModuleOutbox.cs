using System.Data;
using System.Text.Json;
using Dapper;
using Identity.Domain.Contracts;
using Identity.Domain.Contracts.Outbox;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Common;

public sealed class AccountsModuleOutbox(NpgSqlSession session) : IAccountModuleOutbox
{
    private NpgSqlSession Session { get; } = session;
    
    public async Task Add(OutboxMessage message, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO
                           identity_module.outbox
                           (id, type, retry_count, created, sent, payload)
                           VALUES
                           (@id, @type, @retry_count, @created, @sent, @payload::jsonb)
                           """;

        var parameters = new
        {
            id = message.Id,
            type = message.Type,
            retry_count = message.RetryCount,
            created = message.Created,
            sent = message.Sent.HasValue ? message.Sent.Value : (object?)null,
            payload = JsonSerializer.Serialize(message.Payload)
        };

        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        await Session.Execute(command);
    }

    public async Task<OutboxMessage[]> GetMany(OutboxMessageSpecification spec, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(spec);
        string lockClause = LockClause(spec);
        string limitClause = LimitClause(spec);
        string sql = $"""
                      SELECT m.id as id,
                             m.type as type,
                             m.retry_count as retry_count,
                             m.created as created,
                             m.sent as sent,
                             m.payload as payload
                      FROM identity_module.outbox m
                      {filterSql}
                      {limitClause}
                      {lockClause}
                      """;
        
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        OutboxMessage[] messages = await Session.QueryMultipleUsingReader(command, Map);
        return messages;
    }

    private OutboxMessage Map(IDataReader reader)
    {
        Guid id = reader.GetValue<Guid>("id");
        string type = reader.GetValue<string>("type");
        int retryCount = reader.GetValue<int>("retry_count");
        DateTime created = reader.GetValue<DateTime>("created");
        DateTime? sent = reader.GetNullable<DateTime>("sent");
        string payloadJson = reader.GetValue<string>("payload");
        object payload = JsonSerializer.Deserialize<object>(payloadJson)!;
        return new OutboxMessage(id, type, retryCount, created, sent, payload);
    }

    private string LockClause(OutboxMessageSpecification spec)
    {
        return spec.WithLock.HasValue && spec.WithLock.Value ? "FOR UPDATE" : string.Empty;
    }

    private string LimitClause(OutboxMessageSpecification spec)
    {
        return spec.Limit.HasValue ? $"LIMIT {spec.Limit.Value}" : string.Empty;
    }
    
    private (DynamicParameters parameters, string filterSql) WhereClause(OutboxMessageSpecification spec)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();

        if (!string.IsNullOrWhiteSpace(spec.Type))
        {
            filters.Add("m.type = @type");
            parameters.Add("@type", spec.Type, DbType.String);
        }

        if (spec.CreatedDateTime.HasValue)
        {
            filters.Add("m.created < @created");
            parameters.Add("@created", spec.CreatedDateTime.Value, DbType.DateTime);
        }

        if (spec.SentOnly.HasValue)
        {
            filters.Add("m.sent is not null");
        }

        if (spec.NotSentOnly.HasValue)
        {
            filters.Add("m.sent is null");
        }

        if (spec.SentDateTime.HasValue)
        {
            filters.Add("m.sent is not null AND m.sent <= @sent");
            parameters.Add("@sent", spec.SentDateTime.Value, DbType.DateTime);
        }
        
        if (spec.RetryCountLessThan.HasValue)
        {
            filters.Add("m.retry_count < @retry_count");
            parameters.Add("@retry_count", spec.RetryCountLessThan.Value, DbType.Int32);
        }
        
        return (parameters, filters.Count == 0 ? string.Empty : $"WHERE {string.Join(" AND ", filters)}");
    }
}