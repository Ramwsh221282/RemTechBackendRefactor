namespace Identity.Domain.Sessions;

public class UserSession(UserSessionInfo accessToken, UserSessionInfo refreshToken)
{
    public UserSessionInfo AccessToken { get; } = accessToken;
    public UserSessionInfo RefreshToken { get; } = refreshToken;
}
