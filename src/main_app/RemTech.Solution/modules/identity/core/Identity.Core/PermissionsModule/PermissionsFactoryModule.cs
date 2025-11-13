using RemTech.Functional.Extensions;

namespace Identity.Core.PermissionsModule;

public static class PermissionsFactoryModule
{
    extension(Permission)
    {
        public static Result<Permission> Create(string name, Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return Validation("Название разрешения не указано.");
            if (name.Length > 128) return Validation($"Длина разрешения превышает: {128} символов");
            
            Guid resolvedId = id.Resolved();
            return new Permission(name, resolvedId);
        }
    }

    extension(Guid? id)
    {
        private Guid Resolved()
        {
            return id.HasValue ? id.Value : Guid.NewGuid();
        }
    }
}