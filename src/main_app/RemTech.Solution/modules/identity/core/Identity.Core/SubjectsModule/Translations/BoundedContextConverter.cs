using Identity.Core.PermissionsModule;
using Identity.Core.SubjectsModule.Domain.Permissions;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Identity.Core.TicketsModule;
using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Translations;

public static class BoundedContextConverter
{
    public static Result<SubjectPermission> PermissionToSubjectPermission(Permission permission)
    {
        return new SubjectPermission(permission.Id, permission.Name).Validated();
    }

    public static Result<Permission> SubjectPermissionToPermission(SubjectPermission permission)
    {
        return new Permission(permission.Name, permission.Id);
    }
    
    public static Result<SubjectTicket> TicketToSubjectTicket(Ticket ticket)
    {
        return new SubjectTicket(ticket.Id, ticket.Type, ticket.Active).Validated();
    }

    public static Result<Ticket> SubjectTicketToTicket(SubjectTicket ticket)
    {
        Result<Guid> creatorId = ticket.CreatorId();
        if (creatorId.IsFailure) return creatorId.Error;
        return new Ticket(ticket._id, creatorId, ticket._type, DateTime.UtcNow, true);
    }
}