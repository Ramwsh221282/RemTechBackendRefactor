namespace Users.Module.Models.Features.CreatingNewAccount.Exceptions;

internal sealed class PasswordDoesNotContainLowerCharacterException : Exception
{
    public PasswordDoesNotContainLowerCharacterException()
        : base("Пароль должен содержать одну строчную букву") { }
}
