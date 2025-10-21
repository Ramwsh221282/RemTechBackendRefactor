namespace Identity.Domain.Users.Entities;

public sealed class IdentityUserSession(
    Guid sessionId,
    string sessionRefreshToken,
    string sessionToken,
    bool isExpired
)
{
    public IdentityUserSession()
        : this(Guid.Empty, string.Empty, string.Empty, false) { }

    public Guid SessionId { get; } = sessionId;
    public string SessionRefreshToken { get; } = sessionRefreshToken;
    public string SessionToken { get; } = sessionToken;
    public bool IsExpired { get; } = isExpired;
}
