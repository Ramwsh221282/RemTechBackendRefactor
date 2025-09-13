namespace Users.Module.Features.UserPasswordRecovering.Exceptions;

internal sealed class UserRecoveringPasswordByEmailInvalidException : Exception
{
    public UserRecoveringPasswordByEmailInvalidException()
        : base("Почта для операции восстановления некорректна.") { }
}
