using Identity.Core.PermissionsModule.Events;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.Functional.Extensions;

namespace Identity.Core.PermissionsModule;

public sealed record PermissionSnapshot(Guid Id, string Name);

public sealed record Permission
{
    private Guid Id { get; init; }
    private string Name { get; init; }
    private Optional<NotificationsRegistry> Registry { get; init; }

    public Permission Register()
    {
        Registry.ExecuteOnValue(r => r.Record(new PermissionRegistered(Snapshotted())));
        return this;
    }
    
    public PermissionSnapshot Snapshotted() => new(Id, Name);

    public Permission AddRegistry(NotificationsRegistry registry)
    {
        return this with { Registry = Some(registry) };
    }
    
    internal Permission(string name, Guid id, NotificationsRegistry registry)
    : this(name, id) => Registry = Some(registry);
    
    internal Permission(string name, Guid id) =>
        (Id, Name, Registry) = (id, name, None<NotificationsRegistry>());
}

public static class PermissionsModule
{
    extension(Permission)
    {
        public static Result<Permission> Create(string name, Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(name)) 
                return Validation("Название разрешения не указано.");
            
            if (name.Length > 128)
                return Validation($"Длина разрешения превышает: {128} символов");
            
            Guid resolvedId = id.Resolved();
            return new Permission(name, resolvedId);
        }
    }

    extension(Guid? id)
    {
        public Guid Resolved()
        {
            return id.HasValue ? id.Value : Guid.NewGuid();
        }
    }
}