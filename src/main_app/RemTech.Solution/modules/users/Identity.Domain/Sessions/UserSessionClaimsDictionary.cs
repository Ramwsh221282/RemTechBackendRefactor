using System.Text.Json;

namespace Identity.Domain.Sessions;

public sealed class UserSessionClaimsDictionary
{
    private readonly Dictionary<string, string> _claims;
    private readonly UserSession _session;

    public UserSessionClaimsDictionary(UserSession session)
    {
        _session = session;
        _claims = [];
    }

    public string? GetPropertyInfo(string key)
    {
        InitializeClaims();
        return _claims.ContainsKey(key) ? _claims[key] : null;
    }

    private void InitializeClaims()
    {
        if (_claims.Count > 0)
            return;

        string accessToken = _session.AccessToken.Token;
        if (string.IsNullOrEmpty(accessToken))
            return;

        string payloadJson = new JwtTokenPayload(accessToken).AsJson();
        using JsonDocument document = JsonDocument.Parse(payloadJson);
        foreach (JsonProperty property in document.RootElement.EnumerateObject())
        {
            string name = property.Name;
            string value = property.Value.GetRawText();
            _claims.Add(name, value);
        }
    }
}
