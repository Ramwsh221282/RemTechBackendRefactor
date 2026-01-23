using System.Data;
using Dapper;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Tokens;

public sealed class AccessTokensRepository(NpgSqlSession session) : IAccessTokensRepository
{
	private NpgSqlSession Session { get; } = session;

	public async Task Add(AccessToken token, CancellationToken ct = default)
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
		await Session.Execute(command);
	}

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
		if (token is null)
			return Error.NotFound("Токен доступа не найден.");
		return token;
	}

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
		if (token is null)
			return Error.NotFound("Токен доступа не найден.");
		return token;
	}

	public async Task UpdateTokenExpired(string rawToken, CancellationToken ct = default)
	{
		const string sql = "UPDATE identity_module.access_tokens SET is_expired = TRUE WHERE raw_token = @raw_token";
		object parameters = new { raw_token = rawToken };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		await Session.Execute(command);
	}

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

	public async Task Remove(AccessToken token, CancellationToken ct = default)
	{
		const string sql = "DELETE FROM identity_module.access_tokens WHERE token_id = @token_id";
		object parameters = new { token_id = token.TokenId };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		await Session.Execute(command);
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

	private static string LockClause(bool withLock)
	{
		return withLock ? "FOR UPDATE" : string.Empty;
	}
}
