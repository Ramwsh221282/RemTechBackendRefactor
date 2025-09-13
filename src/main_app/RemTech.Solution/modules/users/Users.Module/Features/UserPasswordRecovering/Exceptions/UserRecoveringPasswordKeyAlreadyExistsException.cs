namespace Users.Module.Features.UserPasswordRecovering.Exceptions;

internal sealed class UserRecoveringPasswordKeyAlreadyExistsException : Exception
{
    public UserRecoveringPasswordKeyAlreadyExistsException()
        : base("Пользователь уже создал заявку на сброс пароля.") { }
}
