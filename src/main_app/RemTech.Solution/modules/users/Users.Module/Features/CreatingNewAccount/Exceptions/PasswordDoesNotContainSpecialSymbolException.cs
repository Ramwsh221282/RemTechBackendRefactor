namespace Users.Module.Features.CreatingNewAccount.Exceptions;

internal sealed class PasswordDoesNotContainSpecialSymbolException : Exception
{
    public PasswordDoesNotContainSpecialSymbolException()
        : base("Пароль должен содержать специальный символ") { }
}
