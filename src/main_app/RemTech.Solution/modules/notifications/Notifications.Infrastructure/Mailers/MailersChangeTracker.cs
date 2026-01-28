using System.Data;
using Dapper;
using Notifications.Core.Mailers;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Notifications.Infrastructure.Mailers;

/// <summary>
/// Трекер изменений почтовых ящиков.
/// </summary>
/// <param name="session">Сессия базы данных для выполнения операций.</param>
public sealed class MailersChangeTracker(NpgSqlSession session)
{
	private NpgSqlSession Session { get; } = session;
	private Dictionary<Guid, Mailer> Tracking { get; } = [];

	/// <summary>
	/// Начинает отслеживание изменений для коллекции почтовых ящиков.
	/// </summary>
	/// <param name="mailers">Коллекция почтовых ящиков для отслеживания.</param>
	public void Track(IEnumerable<Mailer> mailers)
	{
		foreach (Mailer mailer in mailers)
			Tracking.TryAdd(mailer.Id.Value, mailer.Copy());
	}

	/// <summary>
	/// Сохраняет изменения отслеживаемых почтовых ящиков в базе данных.
	/// </summary>
	/// <param name="mailers">Коллекция почтовых ящиков для сохранения изменений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения изменений.</returns>
	public Task Save(IEnumerable<Mailer> mailers, CancellationToken ct = default)
	{
		IEnumerable<Mailer> tracking = GetTrackingMailers(mailers);
		return SaveMailerChanges(tracking, ct);
	}

	private static string WhenClause(int index) => $"WHEN m.id=@id_{index}";

	private async Task SaveMailerChanges(IEnumerable<Mailer> mailers, CancellationToken ct)
	{
		Mailer[] mailersArray = [.. mailers];
		if (mailersArray.Length == 0)
			return;

		List<string> setClauses = [];
		DynamicParameters parameters = new();

		if (mailersArray.Any(m => m.Credentials.Email != Tracking[m.Id.Value].Credentials.Email))
		{
			string clause = string.Join(
				" ",
				mailersArray.Select(
					(m, i) =>
					{
						string paramName = $"@email_{i}";
						parameters.Add(paramName, m.Credentials.Email, DbType.String);
						return $"{WhenClause(i)} THEN {paramName}";
					}
				)
			);
			setClauses.Add($"email = CASE {clause} ELSE email END");
		}

		if (mailersArray.Any(m => m.Credentials.SmtpPassword != Tracking[m.Id.Value].Credentials.SmtpPassword))
		{
			string clause = string.Join(
				" ",
				mailersArray.Select(
					(m, i) =>
					{
						string paramName = $"@smtpPassword_{i}";
						parameters.Add(paramName, m.Credentials.SmtpPassword, DbType.String);
						return $"{WhenClause(i)} THEN {paramName}";
					}
				)
			);
			setClauses.Add($"smtp_password = CASE {clause} ELSE smtp_password END");
		}

		if (setClauses.Count == 0)
			return;

		int index = 0;
		List<Guid> ids = [];
		foreach (Mailer mailer in mailersArray)
		{
			Guid value = mailer.Id.Value;
			string paramName = $"@id_{index}";
			parameters.Add(paramName, value, DbType.Guid);
			ids.Add(value);
			index++;
		}

		parameters.Add("@ids", ids.ToArray());

		string updateClause = string.Join(", ", setClauses);
		string sql = $"""
			UPDATE notifications_module.mailers m
			SET {updateClause}
			WHERE m.id = ANY (@ids)
			""";

		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		await Session.Execute(command);
	}

	private List<Mailer> GetTrackingMailers(IEnumerable<Mailer> mailers)
	{
		List<Mailer> tracking = [];
		foreach (Mailer mailer in mailers)
		{
			if (!Tracking.TryGetValue(mailer.Id.Value, out Mailer? tracked))
				continue;
			tracking.Add(tracked);
		}

		return tracking;
	}
}
