namespace Users.Module.Features.UserPasswordRecoveryConfirmation.Exceptions;

internal sealed class UserRecoveryPasswordTicketNotValidException : Exception
{
    public UserRecoveryPasswordTicketNotValidException()
        : base("Ключ подтверждения сброса пароля некорректный.") { }
}
