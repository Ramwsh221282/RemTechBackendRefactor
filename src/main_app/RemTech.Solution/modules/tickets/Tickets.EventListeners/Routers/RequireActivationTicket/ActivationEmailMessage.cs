namespace Tickets.EventListeners.Routers.RequireActivationTicket;

public sealed class ActivationEmailMessage
{
    private readonly string _tickedId;

    public object Content()
    {
        return new
        {
            subject = "Подтверждение почты",
            body = string.Format(
                """
                Запрос на подтверждение почты.
                Чтобы подтвердить почту, необходимо перейти по ссылке: {frontend_url}/{0}
                """, _tickedId)
        };
    }
    
    public ActivationEmailMessage(string ticketId)
    {
        _tickedId = ticketId;
    }
}