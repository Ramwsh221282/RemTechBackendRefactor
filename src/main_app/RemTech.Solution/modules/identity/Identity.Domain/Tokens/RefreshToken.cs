using System.Security.Cryptography;

namespace Identity.Domain.Tokens;

public sealed class RefreshToken(Guid accountId, string tokenValue, long expiresAt)
{
    public Guid AccountId { get; } = accountId;
    public string TokenValue { get; } = tokenValue;
    public long ExpiresAt { get; private set; } = expiresAt;

    public static RefreshToken CreateNew(Guid accountId, DateTime expiresAt)
    {
        string tokenValue = GenerateRandomString();
        long expires = ((DateTimeOffset)(expiresAt)).ToUnixTimeSeconds();
        return new RefreshToken(accountId, tokenValue, expires);
    }

    public bool IsValid(DateTime date)
    {
        long now = ((DateTimeOffset)(date)).ToUnixTimeSeconds();
        return now <= ExpiresAt;
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