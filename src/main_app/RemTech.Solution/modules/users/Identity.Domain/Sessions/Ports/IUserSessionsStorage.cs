namespace Identity.Domain.Sessions.Ports;

public interface IUserSessionsStorage
{
    public abstract Task Store(UserSession session);
    public abstract Task Remove(UserSession session);
}
