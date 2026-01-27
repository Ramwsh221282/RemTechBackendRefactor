using System.Data;
using Dapper;
using Notifications.Core.PendingEmails;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Notifications.Infrastructure.PendingEmails;

/// <summary>
/// Трекер изменений отложенных писем.
/// </summary>
/// <param name="session">Сессия базы данных PostgreSQL.</param>
public sealed class PendingEmailsChangeTracker(NpgSqlSession session)
{
	/// <summary>
	/// Сессия базы данных PostgreSQL.
	/// </summary>
	public NpgSqlSession Session { get; } = session;

	/// <summary>
	/// Отслеживаемые отложенные письма.
	/// </summary>
	public Dictionary<Guid, PendingEmailNotification> Tracking { get; } = [];

	/// <summary>
	/// Начинает отслеживание изменений для множества отложенных писем.
	/// </summary>
	/// <param name="notifications">Множество отложенных писем для отслеживания.</param>
	public void Track(IEnumerable<PendingEmailNotification> notifications)
	{
		foreach (PendingEmailNotification notification in notifications)
			Tracking.TryAdd(notification.Id, notification.Copy());
	}

	/// <summary>
	/// Сохраняет изменения для множества отложенных писем.
	/// </summary>
	/// <param name="notifications">Множество отложенных писем для сохранения изменений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения изменений.</returns>
	public Task Save(IEnumerable<PendingEmailNotification> notifications, CancellationToken ct = default)
	{
		IEnumerable<PendingEmailNotification> tracking = GetTrackingNotifications(notifications);
		return SavePendingEmailNotificationChanges(tracking, ct);
	}

	private static string WhenClause(int i) => $"WHEN p.id = @id_{i}";

	private async Task SavePendingEmailNotificationChanges(
		IEnumerable<PendingEmailNotification> notifications,
		CancellationToken ct
	)
	{
		PendingEmailNotification[] notificationsArray = [.. notifications];
		if (notificationsArray.Length == 0)
			return;
		List<string> setClauses = [];
		DynamicParameters parameters = new();

		if (notificationsArray.Any(n => n.WasSent != Tracking[n.Id].WasSent))
		{
			string clauses = string.Join(
				" ",
				notificationsArray.Select(
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
		foreach (PendingEmailNotification notification in notificationsArray)
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

	private List<PendingEmailNotification> GetTrackingNotifications(IEnumerable<PendingEmailNotification> notifications)
	{
		List<PendingEmailNotification> result = [];
		foreach (PendingEmailNotification notification in notifications)
		{
			if (Tracking.TryGetValue(notification.Id, out _))
				result.Add(notification);
		}

		return result;
	}
}
