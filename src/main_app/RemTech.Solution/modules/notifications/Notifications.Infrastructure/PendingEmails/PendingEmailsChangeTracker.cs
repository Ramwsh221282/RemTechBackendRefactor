using System.Data;
using Dapper;
using Notifications.Core.PendingEmails;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Notifications.Infrastructure.PendingEmails;

public sealed class PendingEmailsChangeTracker(NpgSqlSession session)
{
	public NpgSqlSession Session { get; } = session;
	public Dictionary<Guid, PendingEmailNotification> Tracking { get; } = [];

	public void Track(IEnumerable<PendingEmailNotification> notifications)
	{
		foreach (PendingEmailNotification notification in notifications)
			Tracking.TryAdd(notification.Id, notification.Copy());
	}

	public async Task Save(IEnumerable<PendingEmailNotification> notifications, CancellationToken ct = default)
	{
		IEnumerable<PendingEmailNotification> tracking = GetTrackingNotifications(notifications);
		await SavePendingEmailNotificationChanges(tracking, ct);
	}

	private async Task SavePendingEmailNotificationChanges(
		IEnumerable<PendingEmailNotification> notifications,
		CancellationToken ct
	)
	{
		if (!notifications.Any())
			return;
		List<string> setClauses = [];
		DynamicParameters parameters = new();

		if (notifications.Any(n => n.WasSent != Tracking[n.Id].WasSent))
		{
			string clauses = string.Join(
				" ",
				notifications.Select(
					(n, i) =>
					{
						string paramName = $"@was_sent_{i}";
						parameters.Add(paramName, n.WasSent, DbType.Boolean);
						return $"{WhenClause(i)} THEN {paramName}";
					}
				)
			);
			setClauses.Add($"was_sent = CASE {clauses} ELSE was_sent END");
		}

		if (setClauses.Count == 0)
			return;

		List<Guid> ids = [];
		int index = 0;
		foreach (PendingEmailNotification notification in notifications)
		{
			string paramName = $"@id_{index}";
			parameters.Add(paramName, notification.Id, DbType.Guid);
			ids.Add(notification.Id);
			index++;
		}

		parameters.Add("@ids", ids.ToArray());

		string updateClause = string.Join(", ", setClauses);
		string sql = $"""
			UPDATE notifications_module.pending_emails p
			SET {updateClause}
			WHERE p.id = ANY (@ids)
			""";

		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		await Session.Execute(command);
	}

	private string WhenClause(int i) => $"WHEN p.id = @id_{i}";

	private IEnumerable<PendingEmailNotification> GetTrackingNotifications(
		IEnumerable<PendingEmailNotification> notifications
	)
	{
		List<PendingEmailNotification> result = [];
		foreach (PendingEmailNotification notification in notifications)
			if (Tracking.TryGetValue(notification.Id, out PendingEmailNotification? tracked))
				result.Add(notification);
		return result;
	}
}
