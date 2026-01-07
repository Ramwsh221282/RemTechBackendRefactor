namespace Identity.Infrastructure.Common;

public sealed class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    
    public void Validate()
    {
        if (string.IsNullOrEmpty(SecretKey))
            throw new InvalidOperationException("Secret key must not be empty.");
        if (string.IsNullOrEmpty(Issuer))
            throw new InvalidOperationException("Issuer must not be empty.");
        if (string.IsNullOrEmpty(Audience))
            throw new InvalidOperationException("Audience must not be empty.");
    }
}