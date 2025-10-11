namespace Users.Module.Features.UserPasswordRecovering.Exceptions;

internal sealed class UnableToDetermineHowToResetPasswordException : Exception
{
    public UnableToDetermineHowToResetPasswordException()
        : base("Не удается определить ни логин, ни почту пользователя для сброса пароля.") { }
}
