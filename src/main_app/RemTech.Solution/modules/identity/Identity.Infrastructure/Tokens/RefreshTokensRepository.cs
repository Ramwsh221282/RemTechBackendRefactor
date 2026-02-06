using System.Data;
using Dapper;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Tokens;

/// <summary>
/// Репозиторий для управления токенами обновления.
/// </summary>
/// <param name="session">Сессия базы данных для выполнения операций.</param>
public sealed class RefreshTokensRepository(NpgSqlSession session) : IRefreshTokensRepository
{
	private NpgSqlSession Session { get; } = session;

	/// <summary>
	/// Проверяет существование токена обновления для заданного идентификатора аккаунта.
	/// </summary>
	/// <param name="accountId">Идентификатор аккаунта.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию проверки существования токена.</returns>
	public Task<bool> Exists(Guid accountId, CancellationToken ct = default)
	{
		const string sql =
			"SELECT EXISTS (SELECT 1 FROM identity_module.refresh_tokens WHERE account_id = @account_id)";
		object parameters = new { account_id = accountId };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.QuerySingleRow<bool>(command);
	}

	/// <summary>
	/// Добавляет токен обновления в репозиторий.
	/// </summary>
	/// <param name="token">Токен обновления для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию добавления токена.</returns>
	public Task Add(RefreshToken token, CancellationToken ct = default)
	{
		const string sql = """
			INSERT INTO identity_module.refresh_tokens
			(account_id, token_value, expires_at, created_at)
			VALUES
			(@account_id, @token_value, @expires_at, @created_at)
			ON CONFLICT (account_id) DO UPDATE SET token_value = @token_value, expires_at = @expires_at, created_at = @created_at;
			""";

		object parameters = new
		{
			account_id = token.AccountId,
			token_value = token.TokenValue,
			expires_at = token.ExpiresAt,
			created_at = token.CreatedAt,
		};

		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.Execute(command);
	}

	/// <summary>
	/// Обновляет токен обновления в репозитории.
	/// </summary>
	/// <param name="token">Токен обновления для обновления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию обновления токена.</returns>
	public Task Update(RefreshToken token, CancellationToken ct = default)
	{
		const string sql = """
			UPDATE identity_module.refresh_tokens 
			SET token_value = @token_value, expires_at = @expires_at, created_at = @created_at
			WHERE account_id = @account_id
			""";

		object parameters = new
		{
			account_id = token.AccountId,
			token_value = token.TokenValue,
			expires_at = token.ExpiresAt,
			created_at = token.CreatedAt,
		};

		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.Execute(command);
	}

	/// <summary>
	/// Находит токен обновления по идентификатору аккаунта.
	/// </summary>
	/// <param name="accountId">Идентификатор аккаунта.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции, содержащий найденный токен обновления или ошибку.</returns>
	public async Task<Result<RefreshToken>> Find(Guid accountId, CancellationToken ct = default)
	{
		const string sql = """
			SELECT 
			    account_id as account_id, 
			    token_value as token_value, 
			    expires_at as expires_at,
			    created_at as created_at
			FROM identity_module.refresh_tokens
			WHERE account_id = @account_id
			""";

		object parameters = new { account_id = accountId };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		RefreshToken? token = await Session.QuerySingleUsingReader(command, Map);
		return token is null ? Error.NotFound("Токен обновления не найден.") : token;
	}

	/// <summary>
	/// Находит токен обновления по его значению.
	/// </summary>
	/// <param name="refreshToken">Значение токена обновления.</param>
	/// <param name="withLock">Флаг, указывающий, следует ли блокировать запись для обновления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции, содержащий найденный токен обновления или ошибку.</returns>
	public async Task<Result<RefreshToken>> Find(
		string refreshToken,
		bool withLock = false,
		CancellationToken ct = default
	)
	{
		string lockClause = withLock ? "FOR UPDATE" : string.Empty;
		string sql = $"""
			SELECT 
			    account_id as account_id, 
			    token_value as token_value, 
			    expires_at as expires_at,
			    created_at as created_at
			FROM identity_module.refresh_tokens
			WHERE token_value = @token_value
			{lockClause}
			""";

		object parameters = new { token_value = refreshToken };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		RefreshToken? token = await Session.QuerySingleUsingReader(command, Map);
		return token is null ? Error.NotFound("Токен обновления не найден.") : token;
	}

	/// <summary>
	/// Удаляет токен обновления из репозитория.
	/// </summary>
	/// <param name="token">Токен обновления для удаления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию удаления токена.</returns>
	public Task Delete(RefreshToken token, CancellationToken ct = default)
	{
		const string sql =
			"DELETE FROM identity_module.refresh_tokens WHERE account_id = @account_id AND token_value = @token_value";
		object parameters = new { account_id = token.AccountId, token_value = token.TokenValue };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.Execute(command);
	}

	/// <summary>
	/// Удаляет токен обновления по идентификатору аккаунта.
	/// </summary>
	/// <param name="accountId">ID аккаунта.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию удаления токена.</returns>
	public Task Delete(Guid accountId, CancellationToken ct = default)
	{
		const string sql = """
			    DELETE FROM identity_module.refresh_tokens
			    WHERE account_id = @account_id
			""";
		object parameters = new { account_id = accountId };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.Execute(command);
	}

	private static RefreshToken Map(IDataReader reader)
	{
		Guid accountId = reader.GetGuid(reader.GetOrdinal("account_id"));
		string tokenValue = reader.GetString(reader.GetOrdinal("token_value"));
		long expiresAt = reader.GetInt64(reader.GetOrdinal("expires_at"));
		long createdAt = reader.GetInt64(reader.GetOrdinal("created_at"));
		return new RefreshToken(accountId, tokenValue, expiresAt, createdAt);
	}
}
