namespace Users.Module.Models.Features.CreatingNewAccount.Exceptions;

internal sealed class PasswordDoesNotContainUpperCharacterException : Exception
{
    public PasswordDoesNotContainUpperCharacterException()
        : base("Пароль должен содержать заглавную букву") { }
}
