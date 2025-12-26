namespace Identity.Domain.Sessions.Ports;

public interface IUserSessionsStorage
{
    Task Store(UserSession session);
    Task Remove(UserSession session);
    Task<bool> Contains(UserSession session);
}
