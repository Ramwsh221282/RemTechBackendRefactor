namespace Users.Module.Features.ChangingEmail.Exceptions;

internal sealed class SendersAreNotAvailableYetException : Exception
{
    public SendersAreNotAvailableYetException()
        : base("Требуется наличие отправитилей писем в приложении.") { }
}
