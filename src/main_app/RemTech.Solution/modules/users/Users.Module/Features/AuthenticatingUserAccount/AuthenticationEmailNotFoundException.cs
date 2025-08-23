namespace Users.Module.Features.AuthenticatingUserAccount;

internal sealed class AuthenticationEmailNotFoundException : Exception
{
    public AuthenticationEmailNotFoundException(string email)
        : base($"Пользователь с почтой {email} не найден.") { }
}
