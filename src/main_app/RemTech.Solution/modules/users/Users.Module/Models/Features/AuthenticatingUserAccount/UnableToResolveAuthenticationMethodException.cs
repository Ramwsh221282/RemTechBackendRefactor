namespace Users.Module.Models.Features.AuthenticatingUserAccount;

internal sealed class UnableToResolveAuthenticationMethodException : Exception
{
    public UnableToResolveAuthenticationMethodException()
        : base("Не удается определить имя или email") { }
}
