namespace Identity.Domain.Sessions.Ports;

public interface IUserSessionChecker
{
    public abstract Task<bool> IsValid(UserSession session);
}
