using Identity.Core.SubjectsModule.Domain.Permissions;

namespace Identity.Core.SubjectsModule.Domain.Subjects;

public sealed record SubjectSnapshot(
    Guid Id,
    string Email,
    string Login,
    string Password,
    DateTime? ActivationDate,
    SubjectPermissionSnapshot[] Permissions);