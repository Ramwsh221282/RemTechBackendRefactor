namespace Identity.Domain.Tokens;

public sealed class AccessToken
{
    public required string RawToken { get; set; }
    public required Guid TokenId { get; set; }
    public required long ExpiresAt { get; set; }
    public required long CreatedAt { get; set; }
    public required string Email { get; set; }
    public required string Login { get; set; }
    public required Guid UserId { get; set; }
    public required IEnumerable<string> Permissions { get; set; }
    public required string RawPermissionsString { get; set; }
    public required bool IsExpired { get; set; }

    public AccessToken()
    {
        
    }
    
    public bool ContainsPermission(string permission) => RawPermissionsString.Contains(permission);
}