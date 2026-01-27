using System.Data;
using Dapper;
using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Contracts.Persistence;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Common;

/// <summary>
/// Реализация шаблона Outbox для модуля аккаунтов.
/// </summary>
/// <param name="session">Сессия для работы с базой данных.</param>
/// <param name="unitOfWork">Единица работы для модуля аккаунтов.</param>
public sealed class AccountsModuleOutbox(NpgSqlSession session, IAccountsModuleUnitOfWork unitOfWork)
	: IAccountModuleOutbox
{
	private NpgSqlSession Session { get; } = session;

	private IAccountsModuleUnitOfWork UnitOfWork { get; } = unitOfWork;

	/// <summary>
	/// Добавляет сообщение в таблицу outbox модуля аккаунтов.
	/// </summary>
	/// <param name="message">Сообщение для добавления в outbox.</param>
	/// <param name="ct">Токен отмены для операции добавления.</param>
	/// <returns>Задача, представляющая асинхронную операцию добавления сообщения.</returns>
	public Task Add(IdentityOutboxMessage message, CancellationToken ct = default)
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
			payload = message.Payload,
		};

		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.Execute(command);
	}

	/// <summary>
	/// Получает сообщения из таблицы outbox модуля аккаунтов по спецификации.
	/// </summary>
	/// <param name="spec">Спецификация для фильтрации сообщений.</param>
	/// <param name="ct">Токен отмены для операции получения сообщений.</param>
	/// <returns>Массив сообщений, соответствующих спецификации.</returns>
	public async Task<IdentityOutboxMessage[]> GetMany(OutboxMessageSpecification spec, CancellationToken ct = default)
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
		IdentityOutboxMessage[] messages = await Session.QueryMultipleUsingReader(command, Map);
		UnitOfWork.Track(messages);
		return messages;
	}

	private static string LockClause(OutboxMessageSpecification spec) =>
		spec.WithLock == true ? "FOR UPDATE" : string.Empty;

	private static string LimitClause(OutboxMessageSpecification spec) =>
		spec.Limit.HasValue ? $"LIMIT {spec.Limit.Value}" : string.Empty;

	private static (DynamicParameters Parameters, string FilterSql) WhereClause(OutboxMessageSpecification spec)
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

	private IdentityOutboxMessage Map(IDataReader reader)
	{
		Guid id = reader.GetValue<Guid>("id");
		string type = reader.GetValue<string>("type");
		int retryCount = reader.GetValue<int>("retry_count");
		DateTime created = reader.GetValue<DateTime>("created");
		DateTime? sent = reader.GetNullable<DateTime>("sent");
		string payloadJson = reader.GetValue<string>("payload");
		return new IdentityOutboxMessage(id, type, retryCount, created, sent, payloadJson);
	}
}
