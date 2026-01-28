using System.Data;
using Dapper;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Tokens;

/// <summary>
/// Репозиторий для управления токенами доступа.
/// </summary>
/// <param name="session">Сессия для взаимодействия с базой данных PostgreSQL.</param>
public sealed class AccessTokensRepository(NpgSqlSession session) : IAccessTokensRepository
{
	private NpgSqlSession Session { get; } = session;

	/// <summary>
	/// Добавляет токен доступа в репозиторий.
	/// </summary>
	/// <param name="token">Токен доступа для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию добавления токена.</returns>
	public Task Add(AccessToken token, CancellationToken ct = default)
	{
		const string sql = """
			INSERT INTO identity_module.access_tokens
			(token_id, raw_token, expires_at, created_at, email, login, user_id, raw_permissions, is_expired)
			VALUES
			(@token_id, @raw_token, @expires_at, @created_at, @email, @login, @user_id, @raw_permissions, @is_expired)
			ON CONFLICT (token_id) DO NOTHING;
			""";

		object parameters = new
		{
			is_expired = token.IsExpired,
			token_id = token.TokenId,
			raw_token = token.RawToken,
			expires_at = token.ExpiresAt,
			created_at = token.CreatedAt,
			email = token.Email,
			login = token.Login,
			user_id = token.UserId,
			raw_permissions = token.RawPermissionsString,
		};

		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.Execute(command);
	}

	/// <summary>
	/// Находит токен доступа по его идентификатору.
	/// </summary>
	/// <param name="tokenId">Идентификатор токена доступа.</param>
	/// <param name="withLock">Флаг, указывающий, следует ли блокировать запись для обновления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции, содержащий найденный токен доступа или ошибку.</returns>
	public async Task<Result<AccessToken>> Find(Guid tokenId, bool withLock = false, CancellationToken ct = default)
	{
		string sql = $"""
			SELECT
			token_id as token_id,
			raw_token as raw_token,
			expires_at as expires_at,
			created_at as created_at,
			email as email,
			login as login,
			user_id as user_id,
			raw_permissions as raw_permissions,
			is_expired as is_expired
			FROM identity_module.access_tokens
			WHERE token_id = @token_id
			{LockClause(withLock)}
			""";

		object parameters = new { token_id = tokenId };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		AccessToken? token = await Session.QuerySingleUsingReader(command, Map);
		return token is null
			? (Result<AccessToken>)Error.NotFound("Токен доступа не найден.")
			: (Result<AccessToken>)token;
	}

	/// <summary>
	/// Находит токен доступа по его значению.
	/// </summary>
	/// <param name="accessToken">Значение токена доступа.</param>
	/// <param name="withLock">Флаг, указывающий, следует ли блокировать запись для обновления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции, содержащий найденный токен доступа или ошибку.</returns>
	public async Task<Result<AccessToken>> Find(
		string accessToken,
		bool withLock = false,
		CancellationToken ct = default
	)
	{
		string sql = $"""
			SELECT
			token_id as token_id,
			raw_token as raw_token,
			expires_at as expires_at,
			created_at as created_at,
			email as email,
			login as login,
			user_id as user_id,
			raw_permissions as raw_permissions,
			is_expired as is_expired
			FROM identity_module.access_tokens
			WHERE raw_token = @raw_token
			{LockClause(withLock)}
			""";

		object parameters = new { raw_token = accessToken };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		AccessToken? token = await Session.QuerySingleUsingReader(command, Map);
		return token is null
			? (Result<AccessToken>)Error.NotFound("Токен доступа не найден.")
			: (Result<AccessToken>)token;
	}

	/// <summary>
	/// Обновляет статус истечения срока действия токена доступа.
	/// </summary>
	/// <param name="rawToken">Значение токена доступа.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public Task UpdateTokenExpired(string rawToken, CancellationToken ct = default)
	{
		const string sql = "UPDATE identity_module.access_tokens SET is_expired = TRUE WHERE raw_token = @raw_token";
		object parameters = new { raw_token = rawToken };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.Execute(command);
	}

	/// <summary>
	/// Находит истекшие токены доступа.
	/// </summary>
	/// <param name="maxCount">Максимальное количество токенов для поиска.</param>
	/// <param name="withLock">Флаг, указывающий, следует ли блокировать записи для обновления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Коллекция найденных истекших токенов доступа.</returns>
	public async Task<IEnumerable<AccessToken>> FindExpired(
		int maxCount = 50,
		bool withLock = false,
		CancellationToken ct = default
	)
	{
		string sql = $"""
			SELECT
			token_id as token_id,
			raw_token as raw_token,
			expires_at as expires_at,
			created_at as created_at,
			email as email,
			login as login,
			user_id as user_id,
			raw_permissions as raw_permissions,
			is_expired as is_expired
			FROM identity_module.access_tokens
			WHERE is_expired is TRUE
			LIMIT @max_count
			FOR UPDATE
			{LockClause(withLock)}
			""";

		object parameters = new { max_count = maxCount };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return await Session.QueryMultipleUsingReader(command, Map);
	}

	/// <summary>
	/// Удаляет заданные токены доступа из репозитория.
	/// </summary>
	/// <param name="tokens">Коллекция токенов доступа для удаления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public async Task Remove(IEnumerable<AccessToken> tokens, CancellationToken ct = default)
	{
		AccessToken[] tokensArray = [.. tokens];
		if (tokensArray.Length == 0)
			return;
		Guid[] ids = [.. tokensArray.Select(t => t.TokenId)];
		const string sql = """
			DELETE FROM identity_module.access_tokens
			WHERE token_id = ANY(@token_ids)
			""";
		object parameters = new { token_ids = ids };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		await Session.Execute(command);
	}

	/// <summary>
	/// Удаляет заданный токен доступа из репозитория.
	/// </summary>
	/// <param name="token">Токен доступа для удаления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public Task Remove(AccessToken token, CancellationToken ct = default)
	{
		const string sql = "DELETE FROM identity_module.access_tokens WHERE token_id = @token_id";
		object parameters = new { token_id = token.TokenId };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.Execute(command);
	}

	private static AccessToken Map(IDataReader reader)
	{
		Guid tokenId = reader.GetGuid(reader.GetOrdinal("token_id"));
		string rawToken = reader.GetString(reader.GetOrdinal("raw_token"));
		long expiresAt = reader.GetInt64(reader.GetOrdinal("expires_at"));
		long createdAt = reader.GetInt64(reader.GetOrdinal("created_at"));
		string email = reader.GetString(reader.GetOrdinal("email"));
		string login = reader.GetString(reader.GetOrdinal("login"));
		Guid userId = reader.GetGuid(reader.GetOrdinal("user_id"));
		bool isExpired = reader.GetBoolean(reader.GetOrdinal("is_expired"));
		string rawPermissions = reader.GetString(reader.GetOrdinal("raw_permissions"));
		string[] permissions = rawPermissions.Split(',');

		return new AccessToken()
		{
			IsExpired = isExpired,
			RawToken = rawToken,
			TokenId = tokenId,
			ExpiresAt = expiresAt,
			Email = email,
			Login = login,
			UserId = userId,
			Permissions = permissions,
			RawPermissionsString = rawPermissions,
			CreatedAt = createdAt,
		};
	}

	private static string LockClause(bool withLock) => withLock ? "FOR UPDATE" : string.Empty;
}
