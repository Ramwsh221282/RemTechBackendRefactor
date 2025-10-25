using System.Text.Json;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Sessions;

public class UserSession(UserSessionInfo accessToken, UserSessionInfo refreshToken)
{
    public UserSessionInfo AccessToken { get; } = accessToken;
    public UserSessionInfo RefreshToken { get; } = refreshToken;

    public UserSession(string accessToken, string refreshToken)
        : this(new UserSessionInfo(accessToken), new UserSessionInfo(refreshToken)) { }

    public Status<string> GetRefreshToken()
    {
        try
        {
            using var document = JsonDocument.Parse(refreshToken.Token);
            var id = document.RootElement.GetProperty("details").GetProperty("token").GetGuid();

            if (id == Guid.Empty)
                return new Error("Invalid refresh token.", ErrorCodes.Unauthorized);

            return id.ToString();
        }
        catch
        {
            return new Error("Invalid refresh token.", ErrorCodes.Unauthorized);
        }
    }
}
