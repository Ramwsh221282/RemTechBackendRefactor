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
        string refreshToken = session.GetRefreshToken();
        TimeSpan expiresIn = CreateExpiresIn();
        await _database.StringSetAsync(refreshToken, refreshToken, expiresIn);
    }

    public async Task<bool> Contains(UserSession session)
    {
        string refreshToken = session.GetRefreshToken();
        string? token = await _database.StringGetAsync(refreshToken);
        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task Remove(UserSession session)
    {
        string refreshToken = session.RefreshToken.Token;
        await _database.KeyDeleteAsync(refreshToken);
    }

    private TimeSpan CreateExpiresIn()
    {
        var current = DateTime.UtcNow;
        var last = current.AddHours(7);
        return last.Subtract(current);
    }
}
