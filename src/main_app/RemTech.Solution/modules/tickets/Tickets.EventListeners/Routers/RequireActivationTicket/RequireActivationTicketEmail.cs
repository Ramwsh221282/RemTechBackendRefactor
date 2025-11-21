using RemTech.Functional.Extensions;

namespace Tickets.EventListeners.Routers.RequireActivationTicket;

public interface RequireActivationTicketEmail
{
    public Result<string> Email();
}