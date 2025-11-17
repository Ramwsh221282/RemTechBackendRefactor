using Tickets.Core.Contracts;

namespace Tickets.Core.UseCases;

public static class RegisterTicketUseCase
{
    public static RegisterTicket RegisterTicket => args =>
    {
        Guid ticketId = Guid.NewGuid();
        Guid creatorId = args.CreatorId;
        DateTime created = DateTime.UtcNow;
        string type = args.Type;
        
        Result<TicketLifecycle> lifecycle = TicketLifecycle.Create(ticketId, created, null, new TicketStatus.Created());
        Result<TicketMetadata> metadata = TicketMetadata.Create(creatorId, ticketId, type);
        Result<Ticket> ticket = Ticket.Create(metadata, lifecycle);
        return Task.FromResult(ticket);
    };
}