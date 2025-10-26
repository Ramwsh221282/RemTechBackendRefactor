using System.Text.Json;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Sessions;

public class UserSession
{
    public UserSessionInfo AccessToken { get; }
    public UserSessionInfo RefreshToken { get; }

    public UserSession(UserSessionInfo accessToken, UserSessionInfo refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public UserSession(string accessToken, string refreshToken)
        : this(new UserSessionInfo(accessToken), new UserSessionInfo(refreshToken)) { }

    public Status<string> GetRefreshToken()
    {
        Status<string>[] statuses = [TryGetTokenFromGuid(), TryGetTokenFromJson()];
        if (statuses.All(s => s.IsFailure))
            return statuses.First();

        Status<string> success = statuses.First(s => s.IsSuccess);
        return success;
    }

    private Status<string> TryGetTokenFromGuid()
    {
        string token = RefreshToken.Token;
        return !Guid.TryParse(token, out var guid)
            ? new Error("Invalid refresh token.", ErrorCodes.Unauthorized)
            : guid.ToString();
    }

    private Status<string> TryGetTokenFromJson()
    {
        try
        {
            using var document = JsonDocument.Parse(RefreshToken.Token);
            var id = document.RootElement.GetProperty("details").GetProperty("token").GetGuid();
            return id == Guid.Empty
                ? new Error("Invalid refresh token.", ErrorCodes.Unauthorized)
                : id.ToString();
        }
        catch
        {
            return new Error("Invalid refresh token.", ErrorCodes.Unauthorized);
        }
    }
}
