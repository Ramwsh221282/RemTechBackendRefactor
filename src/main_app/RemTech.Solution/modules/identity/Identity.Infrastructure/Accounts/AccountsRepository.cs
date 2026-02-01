using System.Data;
using System.Data.Common;
using Dapper;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Permissions;
using Npgsql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Accounts;

/// <summary>
/// Репозиторий аккаунтов.
/// </summary>
/// <param name="session">Сессия базы данных PostgreSQL.</param>
/// <param name="unitOfWork">Единица работы для модуля аккаунтов.</param>
public sealed class AccountsRepository(NpgSqlSession session, IAccountsModuleUnitOfWork unitOfWork)
	: IAccountsRepository
{
	private NpgSqlSession Session { get; } = session;

	private IAccountsModuleUnitOfWork UnitOfWork { get; } = unitOfWork;

	/// <summary>
	/// Добавляет новый аккаунт в репозиторий.
	/// </summary>
	/// <param name="account">Аккаунт для добавления.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public Task Add(Account account, CancellationToken ct = default)
	{
		const string sql = """
			INSERT INTO identity_module.accounts
			(id, email, password, login, activation_status)
			VALUES
			(@id, @email, @password, @login, @activation_status)
			""";

		CommandDefinition command = Session.FormCommand(sql, GetParameters(account), ct);
		return Session.Execute(command);
	}

	/// <summary>
	/// Проверяет существование аккаунта по заданной спецификации.
	/// </summary>
	/// <param name="specification">Спецификация для фильтрации аккаунтов.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию с результатом проверки существования аккаунта.</returns>
	public Task<bool> Exists(AccountSpecification specification, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string filterSql) = WhereClause(specification);
		string sql = $"SELECT EXISTS (SELECT 1 FROM identity_module.accounts a {filterSql})";
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.QuerySingleRow<bool>(command);
	}

	/// <summary>
	/// Находит аккаунт по заданной спецификации.
	/// </summary>
	/// <param name="specification">Спецификация для фильтрации аккаунтов.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию с результатом поиска аккаунта.</returns>
	async Task<Result<Account>> IAccountsRepository.Find(AccountSpecification specification, CancellationToken ct)
	{
		return string.IsNullOrWhiteSpace(specification.RefreshToken)
			? await Get(specification, ct)
			: await SearchWithRefreshTokenFilter(specification, ct);
	}

	private static object GetParameters(Account account)
	{
		return new
		{
			id = account.Id.Value,
			email = account.Email.Value,
			password = account.Password.Value,
			login = account.Login.Value,
			activation_status = account.ActivationStatus.Value,
		};
	}

	private static (DynamicParameters Parameters, string FilterSql) WhereClause(AccountSpecification specification)
	{
		DynamicParameters parameters = new();
		List<string> filters = [];

		if (!string.IsNullOrWhiteSpace(specification.RefreshToken))
		{
			parameters.Add("@token_value", specification.RefreshToken, DbType.String);
			// Filter is already included in sql string of method SearchWithRefreshToken()
		}

		if (specification.Id.HasValue)
		{
			parameters.Add("@accountId", specification.Id.Value, DbType.Guid);
			filters.Add("a.id=@accountId");
		}

		if (!string.IsNullOrWhiteSpace(specification.Login))
		{
			parameters.Add("@login", specification.Login, DbType.String);
			filters.Add("a.login=@login");
		}

		if (!string.IsNullOrWhiteSpace(specification.Email))
		{
			parameters.Add("@email", specification.Email, DbType.String);
			filters.Add("a.email=@email");
		}

		return (parameters, filters.Count == 0 ? string.Empty : $"WHERE {string.Join(" AND ", filters)}");
	}

	private async Task<Result<Account>> SearchWithRefreshTokenFilter(
		AccountSpecification specification,
		CancellationToken ct
	)
	{
		(DynamicParameters parameters, string filterSql) = WhereClause(specification);
		string sql = $"""
			WITH refresh_tokens AS (
			SELECT rt.account_id as rt_account_id
			FROM identity_module.refresh_tokens rt
			WHERE rt.token_value = @token_value
			LIMIT 1
			),
			accounts AS (
			        SELECT
			        a.id as account_id,
			        a.email as account_email,
			        a.password as account_password,
			        a.login as account_login,
			        a.activation_status as account_activation_status,
			        ap.permission_id as account_permission_id
			        FROM identity_module.accounts a
			        LEFT JOIN identity_module.account_permissions ap ON ap.account_id=a.id
			        {filterSql}
			        )
			        SELECT 
			        accounts.*, 
			        p.name as permission_name, 
			        p.description as permission_description,
			        rt.rt_account_id as refresh_token_account_id
			        FROM accounts
			        LEFT JOIN identity_module.permissions p ON p.id = accounts.account_permission_id
			        INNER JOIN refresh_tokens rt ON rt.rt_account_id = accounts.account_id
			""";

		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		Account? account = await GetSingle(command, ct);

		if (account is null)
		{
			return Error.NotFound("Учетная запись не найдена.");
		}

		if (specification.LockRequired)
		{
			await BlockEntity(account, ct);
		}

		UnitOfWork.Track([account]);
		return account;
	}

	private async Task<Result<Account>> Get(AccountSpecification specification, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string filterSql) = WhereClause(specification);
		string sql = $"""
			WITH accounts AS (
			SELECT
			a.id as account_id,
			a.email as account_email,
			a.password as account_password,
			a.login as account_login,
			a.activation_status as account_activation_status,
			ap.permission_id as account_permission_id
			FROM identity_module.accounts a
			LEFT JOIN identity_module.account_permissions ap ON ap.account_id=a.id
			{filterSql}
			)
			SELECT accounts.*, p.name as permission_name, p.description as permission_description
			FROM accounts
			LEFT JOIN identity_module.permissions p ON p.id = accounts.account_permission_id
			""";

		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		Account? account = await GetSingle(command, ct);

		if (account is null)
		{
			return Error.NotFound("Учетная запись не найдена.");
		}

		if (specification.LockRequired)
		{
			await BlockEntity(account, ct);
		}

		UnitOfWork.Track([account]);
		return account;
	}

	private async Task<Account?> GetSingle(CommandDefinition command, CancellationToken ct)
	{
		NpgsqlConnection connection = await Session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		Dictionary<Guid, Account> mappings = [];

		while (await reader.ReadAsync(ct))
		{
			Guid id = reader.GetValue<Guid>("account_id");
			if (!mappings.TryGetValue(id, out Account? account))
			{
				string email = reader.GetValue<string>("account_email");
				string password = reader.GetValue<string>("account_password");
				string login = reader.GetValue<string>("account_login");
				bool status = reader.GetValue<bool>("account_activation_status");
				AccountId accountId = AccountId.Create(id);
				account = new Account(
					accountId,
					AccountEmail.Create(email),
					AccountPassword.Create(password),
					AccountLogin.Create(login),
					AccountActivationStatus.Create(status),
					AccountPermissionsCollection.Empty(accountId)
				);
				mappings.Add(id, account);
			}

			if (await reader.IsDBNullAsync(reader.GetOrdinal("account_permission_id"), ct))
			{
				continue;
			}

			Guid permissionId = reader.GetValue<Guid>("account_permission_id");
			string permissionName = reader.GetValue<string>("permission_name");
			string permissionDescription = reader.GetValue<string>("permission_description");

			Permission permission = new(
				PermissionId.Create(permissionId),
				PermissionName.Create(permissionName),
				PermissionDescription.Create(permissionDescription)
			);

			Result<Unit> result = account.Permissions.Add(permission);
			if (result.IsFailure)
			{
				throw new InvalidOperationException(
					$"Ошибка при добавлении разрешения {permissionId} к учетной записи {id}: {result.Error.Message}"
				);
			}
		}

		return mappings.Count == 0 ? null : mappings.First().Value;
	}

	private async Task BlockEntity(Account account, CancellationToken ct)
	{
		await BlockAccount(account, ct);
		await BlockAccountPermissions(account, ct);
	}

	private Task BlockAccount(Account account, CancellationToken ct)
	{
		const string sql = """
			SELECT a.id FROM identity_module.accounts a
			WHERE a.id = @accountId
			FOR UPDATE OF a
			""";
		DynamicParameters parameters = new();
		parameters.Add("@accountId", account.Id.Value, DbType.Guid);
		CommandDefinition command = new(sql, parameters, transaction: Session.Transaction, cancellationToken: ct);
		return Session.Execute(command);
	}

	private Task BlockAccountPermissions(Account account, CancellationToken ct)
	{
		const string sql = """
			SELECT ap.permission_id FROM identity_module.account_permissions ap
			WHERE ap.account_id = @accountId
			FOR UPDATE OF ap
			""";
		DynamicParameters parameters = new();
		parameters.Add("@accountId", account.Id.Value, DbType.Guid);
		CommandDefinition command = new(sql, parameters, transaction: Session.Transaction, cancellationToken: ct);
		return Session.Execute(command);
	}
}
