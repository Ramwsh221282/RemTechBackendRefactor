namespace Tickets.Core.Contracts;

public class TicketRouterEnvelope(ITicketRouter origin) : ITicketRouter
{
    public string SupportedTicketType { get; } = origin.SupportedTicketType;
    
    public virtual Task<TicketRoutingResult> RoutingMethod(TicketRouting routing)
    {
        return origin.RoutingMethod(routing);
    }
}