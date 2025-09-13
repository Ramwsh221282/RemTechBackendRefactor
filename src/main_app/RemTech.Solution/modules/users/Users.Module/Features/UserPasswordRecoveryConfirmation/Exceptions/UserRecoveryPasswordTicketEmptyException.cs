namespace Users.Module.Features.UserPasswordRecoveryConfirmation.Exceptions;

internal sealed class UserRecoveryPasswordTicketEmptyException : Exception
{
    public UserRecoveryPasswordTicketEmptyException()
        : base("Ключ подтверждения сброса пароля был пустым.") { }
}
