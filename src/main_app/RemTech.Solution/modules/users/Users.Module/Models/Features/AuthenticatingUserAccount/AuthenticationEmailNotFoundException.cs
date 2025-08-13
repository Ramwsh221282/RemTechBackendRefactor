namespace Users.Module.Models.Features.AuthenticatingUserAccount;

internal sealed class AuthenticationEmailNotFoundException : Exception
{
    public AuthenticationEmailNotFoundException(string email)
        : base($"Пользователь с почтой {email} не найден.") { }
}
