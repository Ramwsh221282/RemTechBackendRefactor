using System.Data;
using Dapper;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Tokens;

public sealed class RefreshTokensRepository(NpgSqlSession session) : IRefreshTokensRepository
{
	private NpgSqlSession Session { get; } = session;

	public Task<bool> Exists(Guid accountId, CancellationToken ct = default)
	{
		const string sql =
			"SELECT EXISTS (SELECT 1 FROM identity_module.refresh_tokens WHERE account_id = @account_id)";
		object parameters = new { account_id = accountId };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.QuerySingleRow<bool>(command);
	}

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

	public Task Delete(RefreshToken token, CancellationToken ct = default)
	{
		const string sql =
			"DELETE FROM identity_module.refresh_tokens WHERE account_id = @account_id AND token_value = @token_value";
		object parameters = new { account_id = token.AccountId, token_value = token.TokenValue };
		CommandDefinition command = Session.FormCommand(sql, parameters, ct);
		return Session.Execute(command);
	}

	public Task Delete(Guid AccountId, CancellationToken ct = default)
	{
		const string sql = """
			    DELETE FROM identity_module.refresh_tokens
			    WHERE account_id = @account_id
			""";
		object parameters = new { account_id = AccountId };
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
