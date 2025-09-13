namespace Users.Module.Features.UserPasswordRecovering.Exceptions;

internal sealed class UserRecoveringPasswordByLoginEmptyException : Exception
{
    public UserRecoveringPasswordByLoginEmptyException()
        : base("Не удается распознать псевдоним пользователя.") { }
}
