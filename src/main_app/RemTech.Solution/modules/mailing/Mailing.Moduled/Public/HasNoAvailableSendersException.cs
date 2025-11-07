namespace Mailing.Moduled.Public;

public sealed class HasNoAvailableSendersException : Exception
{
    public HasNoAvailableSendersException()
        : base("На текущий момент администратор приложения не настроил почтовых отправителей.")
    {
    }
}