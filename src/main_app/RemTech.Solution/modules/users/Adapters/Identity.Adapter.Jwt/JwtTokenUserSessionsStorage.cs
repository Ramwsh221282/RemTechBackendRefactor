using System.Text.Json;
using Identity.Domain.Sessions;
using Identity.Domain.Sessions.Ports;
using Shared.Infrastructure.Module.Redis;
using StackExchange.Redis;

namespace Identity.Adapter.Jwt;

public sealed class JwtTokenUserSessionsStorage(RedisCache cache) : IUserSessionsStorage
{
    private readonly IDatabase _database = cache.Database;

    public async Task Store(UserSession session)
    {
        string refreshToken = session.RefreshToken.Token;
        TimeSpan expiresIn = CreateExpiresIn(refreshToken);
        await _database.StringSetAsync(refreshToken, refreshToken, expiresIn);
    }

    public async Task<bool> Contains(UserSession session)
    {
        string refreshToken = session.RefreshToken.Token;
        return await _database.KeyExistsAsync(refreshToken);
    }

    public async Task Remove(UserSession session)
    {
        string refreshToken = session.RefreshToken.Token;
        await _database.KeyDeleteAsync(refreshToken);
    }

    private TimeSpan CreateExpiresIn(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return DateTime.UtcNow.Subtract(DateTime.UtcNow);

        if (!refreshToken.Contains(TokenConstants.RefreshToken))
            return DateTime.UtcNow.Subtract(DateTime.UtcNow);

        using JsonDocument document = JsonDocument.Parse(refreshToken);
        var expires = document
            .RootElement.GetProperty("details")
            .GetProperty("expires")
            .GetString();

        if (string.IsNullOrWhiteSpace(expires))
            return DateTime.UtcNow.Subtract(DateTime.UtcNow);

        if (!DateTime.TryParse(expires, out DateTime expiresDate))
            return DateTime.UtcNow.Subtract(DateTime.UtcNow);

        return expiresDate.Subtract(DateTime.UtcNow);
    }
}
