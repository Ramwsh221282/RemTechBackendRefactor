using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Subjects.Events;

namespace Identity.Core.SubjectsModule.Domain.Tickets;

public sealed record SubjectTicket
{
    internal Required<Guid> _creatorId { get; init; }
    internal Guid _id { get; init; }
    internal string _type { get; init; }
    internal bool _active { get; init; }

    public SubjectTicket SignBy(Subject subject)
    {
        Guid creatorId = subject.Snapshot().Id;;
        return this with { _creatorId = Sign(creatorId) };
    }

    public SubjectCreatedTicket Raise()
    {
        Optional<Guid> creatorId = _creatorId.MapOptional(i => i);
        return creatorId.NoValue 
            ? throw new InvalidOperationException(
                $"Cannot raise: {nameof(SubjectCreatedTicket)} from {nameof(SubjectTicket)} when creator id is empty.") 
            : new SubjectCreatedTicket(_id, creatorId.Value, _type);
    }
    
    public bool BelongsTo(Dictionary<string, SubjectTicket> dictionary)
    {
        return dictionary.ContainsKey(_type);
    }

    public void AppendTo(Dictionary<string, SubjectTicket> dictionary)
    {
        dictionary.Add(_type, this);
    }

    public void LeaveFrom(Dictionary<string, SubjectTicket> dictionary)
    {
        dictionary.Remove(_type);
    }
    
    public SubjectTicket Signed(Guid id)
    {
        return this with { _creatorId = Sign(id) };
    }

    public async Task<Result<Unit>> SaveTo(SubjectTicketsStorage storage, CancellationToken ct)
    {
        return await storage.Insert(this, ct);
    }

    public Result<Guid> CreatorId()
    {
        return _creatorId.Map(i => i, Conflict("Заявка должна быть подписана создателем."));
    }

    public SubjectTicketSnapshot Snapshot()
    {
        Optional<Guid> creatorId = _creatorId.MapOptional(i => i);
        return new SubjectTicketSnapshot(creatorId, _id,  _type, _active);
    }
    
    public SubjectTicket(Guid subjectId, Guid id, string type, bool active) 
        :  this(id, type, active) => _creatorId = Sign(subjectId);
    
    public SubjectTicket(Guid id, string type, bool active) =>
        (_id, _type, _active, _creatorId) = (id, type, active, Unsigned<Guid>());

    internal SubjectTicket() : this(Guid.Empty, string.Empty, false) { }
}