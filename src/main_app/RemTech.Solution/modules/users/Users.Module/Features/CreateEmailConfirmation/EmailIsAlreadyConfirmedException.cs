namespace Users.Module.Features.CreateEmailConfirmation;

internal sealed class EmailIsAlreadyConfirmedException : Exception
{
    public EmailIsAlreadyConfirmedException()
        : base("Почта уже подтверждена.") { }
}
