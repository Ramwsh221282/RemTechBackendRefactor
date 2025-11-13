using Identity.Core.SubjectsModule.Models;
using RemTech.Functional.Extensions;

namespace Identity.Core.TicketsModule;

public static class TicketType
{
    public static readonly string AccountActivation = "REGISTRATION_CONFIRMATION";
}

public sealed record TicketSnapshot(
    Guid Id,
    Guid CreatorId,
    string Type,
    DateTime Created,
    DateTime? Closed,
    bool Active);

public sealed record TicketRegistration(Subject Creator, Ticket Ticket);

public sealed record Ticket
{
    internal Guid Id { get; init; }
    internal Guid CreatorId { get; init; }
    internal string Type { get; init; }
    internal DateTime Created { get; init; }
    internal Optional<DateTime> Closed { get; init; }
    internal bool Active { get; init; }
    
    public Result<Ticket> Close(Subject subject)
    {
        if (CreatorId != subject.Snapshotted().Id) 
            return Conflict("Заявка не принадлежит этому пользователю.");
        DateTime closeDate = DateTime.UtcNow;
        return new Ticket(Id, CreatorId, Type, Created, closeDate, false);
    }

    public TicketSnapshot Snapshotted()
    {
        return new TicketSnapshot(
            Id, 
            CreatorId, 
            Type, 
            Created, 
            Closed.HasValue ? Closed.Value : null,
            Active);
    }
    
    internal Ticket(Guid id, Guid creatorId, string type, DateTime created, bool active)
    : this(id, creatorId, type, created, None<DateTime>(), active) { }
    internal Ticket(Guid id, Guid creatorId, string type, DateTime created, DateTime closed, bool active)
    : this(id, creatorId, type, created, Some(closed), active) { }
    internal Ticket(Guid id, Guid creatorId, string type, DateTime created, Optional<DateTime> closed, bool active) =>
        (Id, CreatorId, Type, Created, Closed, Active) = (id, creatorId, type, created, closed, active);
}

public static class TicketsFactoryModule
{
    extension(Ticket)
    {
        public static Ticket New(Guid creatorId, string type)
        {
            return new Ticket(Guid.NewGuid(), creatorId, type, DateTime.UtcNow, true);
        }

        public static Ticket Create(TicketSnapshot snapshot)
        {
            return new Ticket(
                snapshot.Id, 
                snapshot.CreatorId, 
                snapshot.Type, 
                snapshot.Created, 
                FromNullable(snapshot.Closed), 
                snapshot.Active);
        }
    }
}