using Identity.Core.SubjectsModule.Models;

namespace Identity.Core.SubjectsModule.Contexts;

public sealed record SubjectPermissionsConstructionContext(IEnumerable<IdentitySubjectPermission> Permissions);