using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.TicketsModule.Contracts;

namespace Identity.Core.TicketsModule;

public sealed record Ticket
{
    internal Guid Id { get; init; }
    internal Guid CreatorId { get; init; }
    internal string Type { get; init; }
    internal DateTime Created { get; init; }
    internal Optional<DateTime> Closed { get; init; }
    internal bool Active { get; init; }

    public async Task<Result<Unit>> SaveTo(TicketsStorage storage, CancellationToken ct)
    {
        return await storage.Insert(this, ct);
    }
    
    public Result<Ticket> Close(Subject subject)
    {
        if (CreatorId != subject.Snapshot().Id) 
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