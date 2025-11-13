using Identity.Core.PermissionsModule.Events;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.Functional.Extensions;

namespace Identity.Core.PermissionsModule;

public sealed record Permission
{
    internal Guid Id { get; init; }
    internal string Name { get; init; }
    private Optional<NotificationsRegistry> Registry { get; init; }

    public Permission Register()
    {
        Registry.ExecuteOnValue(r => r.Record(new PermissionRegistered(Snapshot())));
        return this;
    }
    
    public PermissionSnapshot Snapshot() => new(Id, Name);

    public Permission BindRegistry(NotificationsRegistry registry)
    {
        return this with { Registry = Some(registry) };
    }

    public Result<Permission> Rename(string otherName)
    {
        Result<Permission> renamed = Permission.Create(otherName, Id);
        if (renamed.IsFailure) return renamed.Error;
        Registry.ExecuteOnValue(r => r.Record(new PermissionRenamed(renamed.Value.Snapshot())));
        return renamed;
    }
    
    internal Permission(string name, Guid id, NotificationsRegistry registry)
    : this(name, id) => Registry = Some(registry);
    
    internal Permission(string name, Guid id) =>
        (Id, Name, Registry) = (id, name, None<NotificationsRegistry>());
}