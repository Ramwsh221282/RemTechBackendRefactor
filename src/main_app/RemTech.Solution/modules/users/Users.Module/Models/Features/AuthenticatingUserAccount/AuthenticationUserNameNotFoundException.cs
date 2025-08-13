namespace Users.Module.Models.Features.AuthenticatingUserAccount;

internal sealed class AuthenticationUserNameNotFoundException : Exception
{
    public AuthenticationUserNameNotFoundException(string name)
        : base($"Пользователь с псевдонимом {name} не найден.") { }
}
