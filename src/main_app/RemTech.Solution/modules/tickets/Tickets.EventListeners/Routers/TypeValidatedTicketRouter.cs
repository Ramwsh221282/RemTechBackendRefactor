using Tickets.Core.Contracts;

namespace Tickets.EventListeners.Routers;

public sealed class TypeValidatedTicketRouter : TicketRouterEnvelope
{
    public override Task<TicketRoutingResult> RoutingMethod(TicketRouting routing) =>
        !routing.Ticket.OfType(SupportedTicketType) 
            ? Task.FromResult(new TicketRoutingResult("Ticket type is not supported.", false)) 
            : base.RoutingMethod(routing);

    public TypeValidatedTicketRouter(ITicketRouter origin) : base(origin)
    {
        
    }
}