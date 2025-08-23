namespace Users.Module.Features.ChangingEmail.Exceptions;

internal sealed class ConfirmationEmailExpiredException : Exception
{
    public ConfirmationEmailExpiredException()
        : base("Срок подтверждения почты закончился. Необходимо повторить процедуру регистрации.")
    { }
}
