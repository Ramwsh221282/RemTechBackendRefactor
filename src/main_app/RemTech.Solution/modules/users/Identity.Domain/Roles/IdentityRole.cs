using Identity.Domain.Roles.Events;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Events;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Roles;

public sealed class IdentityRole
{
    private readonly List<IDomainEvent> _events = [];
    public RoleId Id { get; }
    public RoleName Name { get; }

    public IdentityRole(RoleId id, RoleName name)
    {
        Id = id;
        Name = name;
    }

    public IdentityUserRoleEventArgs ToEventArgs() =>
        new IdentityUserRoleEventArgs(Id.Value, Name.Value);

    public async Task<Status> PublishEvents(
        IDomainEventsDispatcher handler,
        CancellationToken ct = default
    ) => await handler.Dispatch(_events, ct);

    public static IdentityRole Create(RoleName name)
    {
        RoleId id = new RoleId();
        IdentityRole identityRole = new IdentityRole(id, name);
        identityRole._events.Add(new RoleCreatedEvent(id.Value, name.Value));
        return identityRole;
    }
}
