namespace Users.Module.Models.Features.CreatingNewAccount.Exceptions;

internal sealed class PasswordDoesNotContainSpecialSymbolException : Exception
{
    public PasswordDoesNotContainSpecialSymbolException()
        : base("Пароль должен содержать специальный символ") { }
}
