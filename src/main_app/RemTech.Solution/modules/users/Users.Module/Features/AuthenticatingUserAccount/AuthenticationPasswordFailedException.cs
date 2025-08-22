namespace Users.Module.Features.AuthenticatingUserAccount;

internal sealed class AuthenticationPasswordFailedException : Exception
{
    public AuthenticationPasswordFailedException()
        : base("Пароль неверный.") { }
}
