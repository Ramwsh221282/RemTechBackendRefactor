using Identity.Core.SubjectsModule.Domain.Permissions;
using Identity.Core.SubjectsModule.Domain.Tickets;

namespace Identity.Core.SubjectsModule.Domain.Metadata;

public sealed record SubjectMetadata(Guid Id, string Login)
{
    public SubjectTicket Sign(SubjectTicket ticket)
    {
        return ticket.Signed(Id);
    }

    public SubjectPermission Sign(SubjectPermission permission)
    {
        return permission with { Id = Id };
    }
}