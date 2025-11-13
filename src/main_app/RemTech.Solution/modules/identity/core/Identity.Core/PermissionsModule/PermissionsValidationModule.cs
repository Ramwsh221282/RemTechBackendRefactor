using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions;

namespace Identity.Core.PermissionsModule;

public static class PermissionsValidationModule
{
    public const int MaxNameLength = 128;
    
    extension(Permission permission)
    {
        public Result<Permission> Validated()
        {
            Guid id = permission.Id;
            string name = permission.Name;
            if (Guids.Empty(id)) return Conflict("Идентификатор разрешения не указан.");
            if (Strings.EmptyOrWhiteSpace(name)) return Conflict("Название разрешения не указано.");
            if (Strings.GreaterThan(name, MaxNameLength)) return Conflict("Разрешение невалидно.");
            return permission;
        }
    }
}