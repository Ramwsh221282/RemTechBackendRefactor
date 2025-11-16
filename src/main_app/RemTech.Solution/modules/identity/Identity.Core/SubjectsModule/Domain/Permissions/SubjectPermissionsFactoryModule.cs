namespace Identity.Core.SubjectsModule.Domain.Permissions;

public static class SubjectPermissionsFactoryModule
{
    extension(SubjectPermissions)
    {
        public static Result<SubjectPermission> Create(string name, Guid id)
        {
            return new SubjectPermission(id, name).Validated();
        }

        public static SubjectPermissions Empty()
        {
            return new SubjectPermissions([]);
        }
        
        public static Result<SubjectPermissions> Create(IEnumerable<SubjectPermission> permissions)
        {
            Result<IEnumerable<SubjectPermission>> validated = permissions.Validated();
            if (validated.IsFailure) return validated.Error;
            return new SubjectPermissions(permissions);
        }
    }
}