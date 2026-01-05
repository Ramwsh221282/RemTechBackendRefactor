using System.Data;
using Dapper;
using Identity.Domain.Contracts.Outbox;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Common.UnitOfWork;

public sealed class IdentityOutboxMessageChangeTracker(NpgSqlSession session)
{
    private Dictionary<Guid, IdentityOutboxMessage> Tracking { get; } = [];
    private NpgSqlSession Session { get; } = session;

    public void Track(IEnumerable<IdentityOutboxMessage> messages)
    {
        foreach (IdentityOutboxMessage message in messages)
            Tracking.TryAdd(message.Id, message.Clone());
    }

    public async Task Save(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct = default)
    {
        await SaveOutboxMessageChanges(messages, ct);
    }

    private async Task SaveOutboxMessageChanges(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct)
    {
        if (!messages.Any()) return;
        
        List<string> setClauses = [];
        DynamicParameters parameters = new();

        if (messages.Any(m => m.RetryCount != Tracking[m.Id].RetryCount))
        {
            string setClause = string.Join(" ", messages.Select((m, i) =>
            {
                string paramName = $"@retry_count_{i}";
                parameters.Add(paramName, m.RetryCount, DbType.Int32);
                return $"{WhenClause(i)} THEN {paramName}";
            }));
            setClauses.Add($"retry_count = CASE {setClause} ELSE retry_count END");
        }

        if (messages.Any(m => m.Sent.HasValue && m.Sent != Tracking[m.Id].Sent))
        {
            string setClause = string.Join(" ", messages.Select((m, i) =>
            {
                string paramName = $"@sent_{i}";
                parameters.Add(paramName, m.Sent.HasValue ? m.Sent.Value : DBNull.Value, DbType.DateTime);
                return $"{WhenClause(i)} THEN {paramName}";
            }));
            setClauses.Add($"sent = CASE {setClause} ELSE sent END");
        }

        if (setClauses.Count == 0) return;

        int index = 0;
        List<Guid> ids = [];
        foreach (IdentityOutboxMessage message in messages)
        {
            string paramName = $"@id_{index}";
            parameters.Add(paramName, message.Id, DbType.Guid);
            ids.Add(message.Id);
            index++;
        }
        
        parameters.Add("@ids", ids.ToArray());
        string updateSql = $"UPDATE identity_module.outbox m SET {string.Join(", ", setClauses)} WHERE m.id = ANY(@ids)";
        CommandDefinition command = Session.FormCommand(updateSql, parameters, ct);
        await Session.Execute(command);
    }
    
    private string WhenClause(int index)
    {
        return $"WHEN m.id = @id_{index}";
    }
}