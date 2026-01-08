using System.Data;
using System.Text.Json;
using Dapper;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using Npgsql;
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
                           (token_id, raw_token, expires_at, created_at, email, login, user_id, raw_permissions)
                           VALUES
                           (@token_id, @raw_token, @expires_at, @created_at, @email, @login, @user_id, @raw_permissions)
                           ON CONFLICT (token_id) DO NOTHING;
                           """;
        
        object parameters = new
        {
            token_id = token.TokenId,
            raw_token = token.RawToken,
            expires_at = token.ExpiresAt,
            created_at = token.CreatedAt,
            email = token.Email,
            login = token.Login,
            user_id = token.UserId,
            raw_permissions = token.RawPermissionsString
        };

        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        await Session.Execute(command);
    }

    public async Task<Result<AccessToken>> Get(Guid tokenId, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT
                           token_id as token_id,
                           raw_token as raw_token,
                           expires_at as expires_at,
                           created_at as created_at,
                           email as email,
                           login as login,
                           user_id as user_id,
                           raw_permissions as raw_permissions
                           FROM identity_module.access_tokens
                           WHERE id = @id
                           """;
        
        object parameters = new { id = tokenId };
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        AccessToken? token = await Session.QuerySingleUsingReader(command, Map);
        if (token is null) return Error.NotFound("Токен доступа не найден.");
        return token;
    }

    public async Task<Result<AccessToken>> Get(string accessToken, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT
                           token_id as token_id,
                           raw_token as raw_token,
                           expires_at as expires_at,
                           created_at as created_at,
                           email as email,
                           login as login,
                           user_id as user_id,
                           raw_permissions as raw_permissions
                           FROM identity_module.access_tokens
                           WHERE raw_token = @raw_token
                           """;
        
        object parameters = new { raw_token = accessToken };
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        AccessToken? token = await Session.QuerySingleUsingReader(command, Map);
        if (token is null) return Error.NotFound("Токен доступа не найден.");
        return token;
    }

    public async Task<Guid?> Remove(string accessToken, CancellationToken ct = default)
    {
        const string sql = """
                           DELETE FROM identity_module.access_tokens
                           WHERE raw_token = @raw_token
                           RETURNING token_id
                           """;
        
        object parameters = new { raw_token = accessToken };
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        NpgsqlConnection connection = await Session.GetConnection(ct);
        return await connection.QueryFirstOrDefaultAsync<Guid?>(command);
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
        string rawPermissions = reader.GetString(reader.GetOrdinal("raw_permissions"));
        string[] permissions = rawPermissions.Split(',');
        
        return new AccessToken()
        {
            RawToken = rawToken,
            TokenId = tokenId,
            ExpiresAt = expiresAt,
            Email = email,
            Login = login,
            UserId = userId,
            Permissions = permissions,
            RawPermissionsString = rawPermissions,
            CreatedAt = createdAt
        };
    }
}