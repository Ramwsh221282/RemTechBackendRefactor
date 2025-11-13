using Identity.Core.PermissionsModule;
using Identity.Core.SubjectsModule.Domain.Permissions;
using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Translations;

public static class SubjectToPermissionTranslations
{
    public static Result<SubjectPermission> PermissionToSubjectPermission(Permission permission)
    {
        return new SubjectPermission(permission.Id, permission.Name).Validated();
    }

    public static Result<Permission> SubjectPermissionToPermission(SubjectPermission permission)
    {
        return new Permission(permission.Name, permission.Id);
    }
}