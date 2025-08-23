namespace Users.Module.Features.CreateEmailConfirmation;

internal sealed class PasswordInvalidException : Exception
{
    public PasswordInvalidException()
        : base("Пароль неверный.") { }
}
