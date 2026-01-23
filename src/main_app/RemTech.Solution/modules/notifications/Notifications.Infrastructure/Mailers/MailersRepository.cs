using System.Data;
using Dapper;
using Notifications.Core.Common.Contracts;
using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Notifications.Infrastructure.Mailers;

public sealed class MailersRepository(NpgSqlSession session, INotificationsModuleUnitOfWork changeTracker)
	: IMailersRepository
{
	private NpgSqlSession Session { get; } = session;
	private INotificationsModuleUnitOfWork ChangeTracker { get; } = changeTracker;

	public async Task Add(Mailer mailer, CancellationToken ct = default)
	{
		const string sql = """
			INSERT INTO notifications_module.mailers
			    (id, email, smtp_password)
			VALUES
			    (@id, @email, @smtp_password)
			""";
		object parameters = GetParameters(mailer);
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		await Session.Execute(command);
	}

	public async Task<Result<Mailer>> Get(MailersSpecification specification, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string filterSql) = WhereClause(specification);
		string lockClause = LockClause(specification);
		string sql = $"""
			SELECT
			    id as id,
			    email as email,
			    smtp_password as smtp_password
			FROM notifications_module.mailers m
			{filterSql}
			{lockClause}
			LIMIT 1
			""";

		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		Mailer? mailer = await Session.QuerySingleUsingReader(command, Map);
		if (mailer is null)
			return Error.NotFound("Не найдена конфигурация почтового сервиса.");
		ChangeTracker.Track([mailer]);
		return Result.Success(mailer);
	}

	public async Task<Mailer[]> GetMany(MailersSpecification specification, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string filterSql) = WhereClause(specification);
		string lockClause = LockClause(specification);
		string sql = $"""
			SELECT
			    id as id,
			    email as email,
			    smtp_password as smtp_password
			FROM notifications_module.mailers m
			{filterSql}
			{lockClause}
			""";

		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		Mailer[] mailers = await Session.QueryMultipleUsingReader(command, Map);
		ChangeTracker.Track(mailers);
		return mailers;
	}

	public async Task<bool> Exists(MailersSpecification specification, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string filterSql) = WhereClause(specification);
		string sql = $"SELECT EXISTS(SELECT 1 FROM notifications_module.mailers m {filterSql})";
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return await Session.QuerySingleRow<bool>(command);
	}

	public async Task Delete(Mailer mailer, CancellationToken ct = default)
	{
		const string sql = "DELETE FROM notifications_module.mailers WHERE id = @id";
		object parameters = new { id = mailer.Id.Value };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		await Session.Execute(command);
	}

	private static Mailer Map(IDataReader reader)
	{
		Guid id = reader.GetValue<Guid>("id");
		string email = reader.GetValue<string>("email");
		string smtpPassword = reader.GetValue<string>("smtp_password");
		MailerCredentials credentials = MailerCredentials.Create(smtpPassword, email);
		return new Mailer(MailerId.Create(id), credentials);
	}

	private static string LockClause(MailersSpecification specification) =>
		specification.LockRequired.HasValue && specification.LockRequired.Value ? "FOR UPDATE OF m" : string.Empty;

	private static (DynamicParameters parameters, string filterSql) WhereClause(MailersSpecification specification)
	{
		List<string> filters = [];
		DynamicParameters parameters = new();

		if (specification.Id.HasValue)
		{
			filters.Add("m.id = @id");
			parameters.Add("@id", specification.Id.Value, DbType.Guid);
		}

		if (!string.IsNullOrWhiteSpace(specification.Email))
		{
			filters.Add("m.email = @email");
			parameters.Add("@email", specification.Email, DbType.String);
		}

		return (parameters, filters.Count == 0 ? string.Empty : $"WHERE {string.Join(" AND ", filters)}");
	}

	private static object GetParameters(Mailer mailer)
	{
		return new
		{
			id = mailer.Id.Value,
			email = mailer.Credentials.Email,
			smtp_password = mailer.Credentials.SmtpPassword,
		};
	}
}
