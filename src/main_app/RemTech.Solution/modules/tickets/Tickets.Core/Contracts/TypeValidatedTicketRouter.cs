namespace Tickets.Core.Contracts;

public sealed class TypeValidatedTicketRouter(ITicketRouter origin) : TicketRouterEnvelope(origin)
{
    public override Task<TicketRoutingResult> RoutingMethod(TicketRouting routing) =>
        !routing.Ticket.OfType(SupportedTicketType) 
            ? Task.FromResult(new TicketRoutingResult("Ticket type is not supported.", false)) 
            : base.RoutingMethod(routing);
}