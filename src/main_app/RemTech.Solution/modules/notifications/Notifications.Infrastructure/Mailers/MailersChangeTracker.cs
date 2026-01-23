using System.Data;
using Dapper;
using Notifications.Core.Mailers;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Notifications.Infrastructure.Mailers;

public sealed class MailersChangeTracker(NpgSqlSession session)
{
	private NpgSqlSession Session { get; } = session;
	private Dictionary<Guid, Mailer> Tracking { get; } = [];

	public void Track(IEnumerable<Mailer> mailers)
	{
		foreach (Mailer mailer in mailers)
			Tracking.TryAdd(mailer.Id.Value, mailer.Copy());
	}

	public async Task Save(IEnumerable<Mailer> mailers, CancellationToken ct = default)
	{
		IEnumerable<Mailer> tracking = GetTrackingMailers(mailers);
		await SaveMailerChanges(tracking, ct);
	}

	private async Task SaveMailerChanges(IEnumerable<Mailer> mailers, CancellationToken ct)
	{
		if (!mailers.Any())
			return;

		List<string> setClauses = [];
		DynamicParameters parameters = new();

		if (mailers.Any(m => m.Credentials.Email != Tracking[m.Id.Value].Credentials.Email))
		{
			string clause = string.Join(
				" ",
				mailers.Select(
					(m, i) =>
					{
						string paramName = $"@email_{i}";
						parameters.Add(paramName, m.Credentials.Email, DbType.String);
						string whenClause = $"{WhenClause(i)} THEN {paramName}";
						return whenClause;
					}
				)
			);
			setClauses.Add($"email = CASE {clause} ELSE email END");
		}

		if (mailers.Any(m => m.Credentials.SmtpPassword != Tracking[m.Id.Value].Credentials.SmtpPassword))
		{
			string clause = string.Join(
				" ",
				mailers.Select(
					(m, i) =>
					{
						string paramName = $"@smtpPassword_{i}";
						parameters.Add(paramName, m.Credentials.SmtpPassword, DbType.String);
						string whenClause = $"{WhenClause(i)} THEN {paramName}";
						return whenClause;
					}
				)
			);
			setClauses.Add($"smtp_password = CASE {clause} ELSE smtp_password END");
		}

		if (setClauses.Count == 0)
			return;

		int index = 0;
		List<Guid> ids = [];
		foreach (Mailer mailer in mailers)
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

	private IEnumerable<Mailer> GetTrackingMailers(IEnumerable<Mailer> mailers)
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

	private string WhenClause(int index)
	{
		return $"WHEN m.id=@id_{index}";
	}
}
