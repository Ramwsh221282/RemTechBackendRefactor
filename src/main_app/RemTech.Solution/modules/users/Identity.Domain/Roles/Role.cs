using Identity.Domain.Roles.Events;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Events;
using Identity.Domain.Users.Ports.EventHandlers;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Roles;

public sealed class Role
{
    private readonly List<IdentityUserEvent> _events = [];
    public RoleId Id { get; }
    public RoleName Name { get; }

    public Role(RoleId id, RoleName name)
    {
        Id = id;
        Name = name;
    }

    public IdentityUserRoleEventArgs ToEventArgs() =>
        new IdentityUserRoleEventArgs(Id.Value, Name.Value);

    public async Task<Status> PublishEvents(
        IIdentityUserEventHandler handler,
        CancellationToken ct = default
    ) => await handler.Handle(_events, ct);

    public static Role Create(RoleName name)
    {
        RoleId id = new RoleId();
        Role role = new Role(id, name);
        role._events.Add(new RoleCreatedEvent(id.Value, name.Value));
        return role;
    }
}
