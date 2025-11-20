using Tickets.Core.Contracts;

namespace Tickets.Core.UseCases;

public static class RegisterTicketUseCase
{
    public static RegisterTicket RegisterTicket => args =>
    {
        Guid creatorId = args.CreatorId;
        DateTime created = DateTime.UtcNow;
        string type = args.Type;
        string? json = args.ExtraInformationJson;
        
        Result<TicketLifecycle> lifecycle = TicketLifecycle.Create(args.TicketId, created, null, new TicketStatus.Created());
        Result<TicketMetadata> metadata = TicketMetadata.Create(creatorId, args.TicketId, type, json);
        Result<Ticket> ticket = Ticket.Create(metadata, lifecycle);
        return Task.FromResult(ticket);
    };
}