using Identity.Core.SubjectsModule.Models;
using RemTech.Functional.Extensions;

namespace Identity.Core.TicketsModule;

public static class IdentityTicketTypes
{
    public static readonly string AccountActivation = "REGISTRATION_CONFIRMATION";
}

public sealed record IdentityTicketSnapshot(
    Guid Id,
    Guid CreatorId,
    string Type,
    DateTime Created,
    DateTime? Closed);

public sealed record TicketRegistration(Subject Creator, IdentityTicket Ticket);

public sealed record IdentityTicket
{
    internal Guid Id { get; init; }
    internal Guid CreatorId { get; init; }
    internal string Type { get; init; }
    internal DateTime Created { get; init; }
    internal Optional<DateTime> Closed { get; init; }
    internal bool Active { get; init; }
    
    internal IdentityTicket(Guid id, Guid creatorId, string type, DateTime created, bool active)
    {
        Id = id;
        CreatorId = creatorId;
        Type = type;
        Created = created;
        Closed = None<DateTime>();
        Active = active;
    }
    
    internal IdentityTicket(Guid id, Guid creatorId, string type, DateTime created, DateTime closed, bool active)
    {
        Id = id;
        CreatorId = creatorId;
        Type = type;
        Created = created;
        Closed = Some(closed);
        Active = active;
    }

    public Result<IdentityTicket> Close(Subject subject)
    {
        if (CreatorId != subject.Snapshotted().Id) 
            return Conflict("Заявка не принадлежит этому пользователю.");
        
        DateTime closeDate = DateTime.UtcNow;
        return new IdentityTicket(Id, CreatorId, Type, Created, closeDate);
    }

    public IdentityTicketSnapshot Snapshotted()
    {
        return new IdentityTicketSnapshot(Id, CreatorId, Type, Created, Closed.HasValue ? Closed.Value : null);
    }
}

public static class IdentityTicketFactoryModule
{
    extension(IdentityTicket)
    {
        public static IdentityTicket New(Guid creatorId, string type)
        {
            return new IdentityTicket(Guid.NewGuid(), creatorId, type, DateTime.UtcNow, true);
        }
    }
}