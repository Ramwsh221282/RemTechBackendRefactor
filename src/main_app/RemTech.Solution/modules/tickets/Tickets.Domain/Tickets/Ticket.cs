using Tickets.Domain.Tickets.ValueObjects;

namespace Tickets.Domain.Tickets;

public sealed class Ticket
{
    public TicketId Id { get; }
    public TicketLifeTime LifeTime { get; }
    public TicketContent Content { get; }

    public Ticket(TicketId id, TicketLifeTime lifeTime, TicketContent content)
    {
        Id = id;
        LifeTime = lifeTime;
        Content = content;
    }
}
