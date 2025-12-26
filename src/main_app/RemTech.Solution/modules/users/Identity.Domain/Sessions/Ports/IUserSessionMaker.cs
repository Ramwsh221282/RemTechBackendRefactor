using Identity.Domain.Users.Aggregate;

namespace Identity.Domain.Sessions.Ports;

public interface IUserSessionMaker
{
    Task<UserSession> Create(User user);
}
