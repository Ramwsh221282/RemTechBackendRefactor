namespace Users.Module.Features.CreatingNewAccount.Exceptions;

internal sealed class PasswordShouldContainDigitException : Exception
{
    public PasswordShouldContainDigitException()
        : base("Пароль должен содержать спец. символ.") { }
}
