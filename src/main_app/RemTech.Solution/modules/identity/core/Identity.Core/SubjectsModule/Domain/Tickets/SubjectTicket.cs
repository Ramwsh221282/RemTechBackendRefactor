using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Domain.Tickets;

public sealed record SubjectTicket
{
    internal Required<Guid> _creatorId { get; init; }
    internal Guid _id { get; init; }
    internal string _type { get; init; }
    internal bool _active { get; init; }

    public SubjectTicket SignBy(Subject subject)
    {
        Guid creatorId = subject._metadata.Id;
        return this with { _creatorId = Sign(creatorId) };
    }
    
    public SubjectTicket(Guid subjectId, Guid id, string type, bool active) 
    :  this(id, type, active) => _creatorId = Sign(subjectId);
    
    public SubjectTicket(Guid id, string type, bool active) =>
        (_id, _type, _active, _creatorId) = (id, type, active, Required.Unsigned<Guid>());

    public Result<Guid> CreatorId()
    {
        return _creatorId.Map(i => i, Conflict("Заявка должна быть подписана создателем."));
    }
    
    public Func<SubjectTicket, bool> OfType() => t => t._type == _type;
    public Func<SubjectTicket, bool> OfId() => t => t._id == _id;
    public Func<SubjectTicket, bool> Active() => t => t._active == _active;
}