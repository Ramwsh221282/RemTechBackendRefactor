namespace Users.Module.Models.Features.CreatingNewAccount.Exceptions;

internal sealed class PasswordLengthIsNotSatisfiedException : Exception
{
    public PasswordLengthIsNotSatisfiedException()
        : base("Длина пароля меньше 8 символов.") { }
}
