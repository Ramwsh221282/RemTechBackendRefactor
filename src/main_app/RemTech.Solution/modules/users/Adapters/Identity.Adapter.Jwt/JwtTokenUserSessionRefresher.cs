using Identity.Domain.Sessions;
using Identity.Domain.Sessions.Ports;
using Identity.Domain.Users.Aggregate;

namespace Identity.Adapter.Jwt;

public sealed class JwtTokenUserSessionRefresher(
    IUserSessionMaker maker,
    IUserSessionsStorage storage
) : IUserSessionRefresher
{
    public async Task<UserSession> Refresh(User user, UserSession session)
    {
        await storage.Remove(session);
        var newSession = await maker.Create(user);
        await storage.Store(newSession);
        return newSession;
    }
}
