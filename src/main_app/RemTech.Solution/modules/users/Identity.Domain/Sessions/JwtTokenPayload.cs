using System.Text;

namespace Identity.Domain.Sessions;

public sealed class JwtTokenPayload(string token)
{
    public JwtTokenPayload(UserSession session)
        : this(session.AccessToken.Token) { }

    public string AsJson()
    {
        if (string.IsNullOrEmpty(token))
            return string.Empty;

        var parts = token.Split('.');
        if (parts.Length != 3)
            return string.Empty;

        string payloadBase64 = parts[1];

        string base64String =
            payloadBase64.Length % 4 == 0
                ? payloadBase64
                : payloadBase64 + new string('=', 4 - payloadBase64.Length % 4);

        base64String = base64String.Replace('-', '+').Replace('_', '/');

        byte[] bytes = Convert.FromBase64String(base64String);
        return Encoding.UTF8.GetString(bytes);
    }
}
