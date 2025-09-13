namespace Users.Module.Features.UserPasswordRecovering.Exceptions;

internal sealed class PasswordResetMessageDetails
{
    private readonly string _message;

    private PasswordResetMessageDetails(string message)
    {
        _message = message;
    }

    public static PasswordResetMessageDetails Standard()
    {
        return new PasswordResetMessageDetails(
            "На указанную почту при регистрации была отправлена инструкция по сбросу пароля."
        );
    }
}
