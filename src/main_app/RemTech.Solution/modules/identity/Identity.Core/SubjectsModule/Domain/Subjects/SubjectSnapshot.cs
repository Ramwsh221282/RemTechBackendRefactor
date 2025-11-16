using Identity.Core.SubjectsModule.Domain.Permissions;
using Identity.Core.SubjectsModule.Domain.Tickets;

namespace Identity.Core.SubjectsModule.Domain.Subjects;

public sealed record SubjectSnapshot(
    Guid Id,
    string Email,
    string Login,
    string Password,
    DateTime? ActivationDate,
    SubjectPermissionSnapshot[] Permissions,
    SubjectTicketSnapshot[] Tickets);