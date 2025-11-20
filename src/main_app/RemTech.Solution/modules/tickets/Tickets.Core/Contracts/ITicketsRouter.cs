namespace Tickets.Core.Contracts;

public interface ITicketRouter
{
    public string SupportedTicketType { get; }
    public Task RouteTicket(Ticket ticket, CancellationToken ct = default);
}