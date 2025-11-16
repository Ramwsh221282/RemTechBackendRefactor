namespace Identity.Core.SubjectsModule.Domain.Permissions;

public static class SubjectPermissionsValidationModule
{
    public const int MaxLength = 128;
    
    extension(SubjectPermission permission)
    {
        internal Result<SubjectPermission> Validated()
        {
            string name = permission.Name;
            Guid id = permission.Id;
            if (Strings.EmptyOrWhiteSpace(name)) return Validation("Название разрешения не указано.");
            if (Strings.GreaterThan(name, MaxLength))
                return Validation($"Название разрешения превышает длину: {MaxLength} символов.");
            if (Guids.Empty(id)) return Validation($"Идентификатор разрешения пустой.");
            return permission;
        }
    }

    extension(IEnumerable<SubjectPermission> permissions)
    {
        internal Result<IEnumerable<SubjectPermission>> Validated()
        {
            HashSet<string> names = [];
            foreach (SubjectPermission permission in permissions)
            {
                if (!names.Add(permission.Name))
                    return Validation("Список разрешений должен быть уникальным.");
            }

            return Success(permissions);
        }
    }
}