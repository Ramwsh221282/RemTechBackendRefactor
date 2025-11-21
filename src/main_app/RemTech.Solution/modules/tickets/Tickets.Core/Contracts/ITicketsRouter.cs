namespace Tickets.Core.Contracts;

public sealed record TicketRouting(Ticket Ticket, CancellationToken Ct);

public sealed record TicketRoutingResult(string Message, bool IsSuccess);

public interface ITicketRouter
{
    string SupportedTicketType { get; }
    Task<TicketRoutingResult> RoutingMethod(TicketRouting routing);
}