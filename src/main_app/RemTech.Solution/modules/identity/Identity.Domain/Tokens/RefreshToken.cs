using System.Security.Cryptography;

namespace Identity.Domain.Tokens;

public sealed class RefreshToken(Guid accountId, string tokenValue, long expiresAt, long createdAt)
{
    public Guid AccountId { get; } = accountId;
    public string TokenValue { get; } = tokenValue;
    public long ExpiresAt { get; private set; } = expiresAt;
    public long CreatedAt { get; private set; } = createdAt;

    public static RefreshToken CreateNew(Guid accountId, long expiresAt, long createdAt)
    {
        string tokenValue = GenerateRandomString();
        return new RefreshToken(accountId, tokenValue, expiresAt, createdAt);
    }

    public bool IsExpired()
    {
        return ExpiresAt > CreatedAt;
    }
    
    private static string GenerateRandomString()
    {
        var randomBytes = new byte[32];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert
            .ToBase64String(randomBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}