using System.Data;
using System.Data.Common;
using Dapper;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tickets;
using Npgsql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Tickets;

/// <summary>
/// Репозиторий для управления заявками аккаунтов.
/// </summary>
/// <param name="session">Сессия базы данных PostgreSQL.</param>
/// <param name="unitOfWork">Единица работы для модуля аккаунтов.</param>
public sealed class AccountTicketsRepository(NpgSqlSession session, IAccountsModuleUnitOfWork unitOfWork)
	: IAccountTicketsRepository
{
	private NpgSqlSession Session { get; } = session;
	private IAccountsModuleUnitOfWork UnitOfWork { get; } = unitOfWork;

	/// <summary>
	/// Добавляет новую заявку аккаунта в репозиторий.
	/// </summary>
	/// <param name="ticket">Заявка аккаунта для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию добавления заявки.</returns>
	public Task Add(AccountTicket ticket, CancellationToken ct = default)
	{
		const string sql = """
			INSERT INTO identity_module.tickets
			(id, creator_id, finished, purpose)
			VALUES
			(@id, @creator_id, @finished, @purpose)
			""";
		object parameters = GetParameters(ticket);
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.Execute(command);
	}

	/// <summary>
	/// Находит одну заявку аккаунта по заданной спецификации.
	/// </summary>
	/// <param name="specification">Спецификация для поиска заявки аккаунта.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат, содержащий найденную заявку аккаунта или ошибку.</returns>
	public async Task<Result<AccountTicket>> Find(
		AccountTicketSpecification specification,
		CancellationToken ct = default
	)
	{
		(DynamicParameters parameters, string filterSql) = WhereClause(specification);
		string sql = $"""
			SELECT
			id as ticket_id,
			creator_id as creator_id,
			finished as finished,
			purpose as purpose
			FROM identity_module.tickets
			{filterSql}
			""";

		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		AccountTicket? ticket = await GetSingle(command, ct);

		if (ticket is null)
		{
			return Error.NotFound("Заявка не найдена.");
		}

		if (specification.LockRequired == true)
		{
			await BlockTicket(ticket, ct);
		}

		UnitOfWork.Track([ticket]);
		return Result.Success(ticket);
	}

	private static (DynamicParameters Parameters, string FilterSql) WhereClause(
		AccountTicketSpecification specification
	)
	{
		DynamicParameters parameters = new();
		List<string> filterSql = [];

		if (specification.TicketId.HasValue)
		{
			parameters.Add("@ticketId", specification.TicketId.Value, DbType.Guid);
			filterSql.Add("id = @ticketId");
		}

		if (specification.AccountId.HasValue)
		{
			parameters.Add("@creatorId", specification.AccountId.Value, DbType.Guid);
			filterSql.Add("creator_id = @creatorId");
		}

		if (specification.Finished.HasValue)
		{
			if (specification.Finished.Value)
			{
				filterSql.Add("finished is true");
			}
			else
			{
				filterSql.Add("finished is false");
			}
		}

		if (!string.IsNullOrWhiteSpace(specification.Purpose))
		{
			parameters.Add("@purpose", specification.Purpose, DbType.String);
			filterSql.Add("purpose=@purpose");
		}

		return (parameters, filterSql.Count == 0 ? string.Empty : $"WHERE {string.Join(" AND ", filterSql)}");
	}

	private static object GetParameters(AccountTicket ticket)
	{
		return new
		{
			id = ticket.TicketId,
			creator_id = ticket.AccountId.Value,
			finished = ticket.Finished,
			purpose = ticket.Purpose,
		};
	}

	private async Task<AccountTicket?> GetSingle(CommandDefinition command, CancellationToken ct)
	{
		Dictionary<Guid, AccountTicket> mappings = [];
		NpgsqlConnection connection = await Session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);

		while (await reader.ReadAsync(ct))
		{
			Guid id = reader.GetValue<Guid>("ticket_id");
			if (!mappings.TryGetValue(id, out _))
			{
				Guid creatorId = reader.GetValue<Guid>("creator_id");
				bool finished = reader.GetValue<bool>("finished");
				string purpose = reader.GetValue<string>("purpose");
				AccountTicket? ticket = new(AccountId.Create(creatorId), id, finished, purpose);
				mappings.Add(id, ticket);
			}
		}

		return mappings.Count == 0 ? null : mappings.First().Value;
	}

	private Task BlockTicket(AccountTicket ticket, CancellationToken ct = default)
	{
		const string sql = "SELECT id FROM identity_module.tickets WHERE id = @ticketId FOR UPDATE";
		DynamicParameters parameters = new();
		parameters.Add("@ticketId", ticket.TicketId, DbType.Guid);
		CommandDefinition command = new(sql, parameters, transaction: Session.Transaction, cancellationToken: ct);
		return Session.Execute(command);
	}
}
