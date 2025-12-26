using Identity.Domain.Users.Aggregate;

namespace Identity.Domain.Sessions.Ports;

public interface IUserSessionRefresher
{
    public Task<UserSession> Refresh(User user, UserSession session);
}
