namespace Users.Module.Models.Features.VerifyingAdmin;

internal sealed class TokensExpiredException : Exception
{
    public TokensExpiredException()
        : base("Expired tokens sessions.") { }
}
