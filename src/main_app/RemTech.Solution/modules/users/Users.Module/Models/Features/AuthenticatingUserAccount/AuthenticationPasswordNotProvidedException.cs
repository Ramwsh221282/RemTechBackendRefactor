namespace Users.Module.Models.Features.AuthenticatingUserAccount;

internal sealed class AuthenticationPasswordNotProvidedException : Exception
{
    public AuthenticationPasswordNotProvidedException()
        : base("Пароль для авторизации не предоставлен") { }
}
