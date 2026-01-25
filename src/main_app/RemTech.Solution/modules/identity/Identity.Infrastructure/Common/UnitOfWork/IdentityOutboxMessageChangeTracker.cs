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

	public Task Save(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct = default)
	{
		IEnumerable<IdentityOutboxMessage> tracking = GetTrackingMessages(messages);
		return SaveOutboxMessageChanges(tracking, ct);
	}

	private static string WhenClause(int index) => $"WHEN m.id = @id_{index}";

	private async Task SaveOutboxMessageChanges(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct)
	{
		IdentityOutboxMessage[] messagesArray = [.. messages];
		if (messagesArray.Length == 0)
			return;

		List<string> setClauses = [];
		DynamicParameters parameters = new();

		if (messagesArray.Any(m => m.RetryCount != Tracking[m.Id].RetryCount))
		{
			string setClause = string.Join(
				" ",
				messagesArray.Select(
					(m, i) =>
					{
						string paramName = $"@retry_count_{i}";
						parameters.Add(paramName, m.RetryCount, DbType.Int32);
						return $"{WhenClause(i)} THEN {paramName}";
					}
				)
			);
			setClauses.Add($"retry_count = CASE {setClause} ELSE retry_count END");
		}

		if (messagesArray.Any(m => m.Sent.HasValue && !Tracking[m.Id].Sent.HasValue))
		{
			string setClause = string.Join(
				" ",
				messagesArray.Select(
					(m, i) =>
					{
						string paramName = $"@sent_{i}";
						parameters.Add(paramName, m.Sent.HasValue ? m.Sent.Value : DBNull.Value, DbType.DateTime);
						return $"{WhenClause(i)} THEN {paramName}";
					}
				)
			);
			setClauses.Add($"sent = CASE {setClause} ELSE sent END");
		}

		if (setClauses.Count == 0)
			return;

		int index = 0;
		List<Guid> ids = [];
		foreach (IdentityOutboxMessage message in messagesArray)
		{
			string paramName = $"@id_{index}";
			parameters.Add(paramName, message.Id, DbType.Guid);
			ids.Add(message.Id);
			index++;
		}

		parameters.Add("@ids", ids.ToArray());
		string updateSql =
			$"UPDATE identity_module.outbox m SET {string.Join(", ", setClauses)} WHERE m.id = ANY(@ids)";
		CommandDefinition command = Session.FormCommand(updateSql, parameters, ct);
		await Session.Execute(command);
	}

	private List<IdentityOutboxMessage> GetTrackingMessages(IEnumerable<IdentityOutboxMessage> messages)
	{
		List<IdentityOutboxMessage> tracking = [];
		foreach (IdentityOutboxMessage message in messages)
		{
			if (!Tracking.TryGetValue(message.Id, out IdentityOutboxMessage? _))
				continue;
			tracking.Add(message);
		}

		return tracking;
	}
}
