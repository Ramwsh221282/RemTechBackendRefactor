using Identity.Domain.Sessions.Ports;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Sessions;

public class UserSessionsService(
    IUserSessionMaker maker,
    IUserSessionsStorage storage,
    IUserSessionChecker checker,
    IUserSessionRefresher refresher,
    IUsersStorage users
)
{
    public async Task<UserSession> Create(User user)
    {
        var session = await maker.Create(user);
        await storage.Store(session);
        return session;
    }

    public async Task Remove(UserSession session)
    {
        await storage.Remove(session);
    }

    public async Task<Status<UserSession>> Refresh(UserSession session)
    {
        if (!await storage.Contains(session))
            return Error.Unauthorized();

        var userId = GetSubjectId(session);
        if (userId.IsFailure)
            return Error.Unauthorized();

        var user = await users.Get(userId);
        return user == null ? Error.Unauthorized() : await refresher.Refresh(user, session);
    }

    public async Task<bool> Validate(UserSession session)
    {
        return await checker.IsValid(session);
    }

    private Status<UserId> GetSubjectId(UserSession session)
    {
        var claims = new UserSessionClaimsDictionary(session);
        return claims.ExtractId();
    }

    private UserSession EmptySession() =>
        new(new UserSessionInfo(string.Empty), new UserSessionInfo(string.Empty));
}
