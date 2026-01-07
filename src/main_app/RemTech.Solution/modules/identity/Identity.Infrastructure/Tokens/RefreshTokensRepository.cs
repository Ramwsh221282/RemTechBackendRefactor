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
    
    public async Task<bool> Exists(Guid accountId, CancellationToken ct = default)
    {
        const string sql = "SELECT EXISTS (SELECT 1 FROM identity_module.refresh_tokens WHERE account_id = @account_id)";
        object parameters = new { account_id = accountId };
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        return await Session.QuerySingleRow<bool>(command);
    }

    public async Task Add(RefreshToken token, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO identity_module.refresh_tokens
                           (account_id, token_value, expires_at)
                           VALUES
                           (@account_id, @token_value, @expires_at)
                           ON CONFLICT (account_id) DO UPDATE SET token_value = @token_value, expires_at = @expires_at;
                           """;

        object parameters = new
        {
            account_id = token.AccountId,
            token_value = token.TokenValue,
            expires_at = token.ExpiresAt
        };

        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        await Session.Execute(command);
    }

    public async Task Update(RefreshToken token, CancellationToken ct = default)
    {
        const string sql = """
                           UPDATE identity_module.refresh_tokens 
                           SET token_value = @value, expires_at = @expires_at
                           WHERE account_id = @account_id
                           """;
        
        object parameters = new
        {
            account_id = token.AccountId,
            value = token.TokenValue,
            expires_at = token.ExpiresAt
        };
        
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        await Session.Execute(command);
    }

    public async Task<Result<RefreshToken>> Get(Guid accountId, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT 
                               account_id as account_id, 
                               token_value as token_value, 
                               expires_at as expires_at
                           FROM identity_module.refresh_tokens
                           WHERE account_id = @account_id
                           """;
        
        object parameters = new { account_id = accountId };
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        RefreshToken? token = await Session.QuerySingleUsingReader(command, Map);
        return token is null ? Error.NotFound("Токен обновления не найден.") : token;
    }

    public async Task<Result<RefreshToken>> Get(string refreshToken, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT 
                               account_id as account_id, 
                               token_value as token_value, 
                               expires_at as expires_at
                           FROM identity_module.refresh_tokens
                           WHERE token_value = @token_value
                           """;
        
        object parameters = new { token_value = refreshToken };
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        RefreshToken? token = await Session.QuerySingleUsingReader(command, Map);
        return token is null ? Error.NotFound("Токен обновления не найден.") : token;
    }

    private static RefreshToken Map(IDataReader reader)
    {
        Guid accountId = reader.GetGuid(reader.GetOrdinal("account_id"));
        string tokenValue = reader.GetString(reader.GetOrdinal("token_value"));
        long expiresAt = reader.GetInt64(reader.GetOrdinal("expires_at"));
        return new RefreshToken(accountId, tokenValue, expiresAt);
    }
}