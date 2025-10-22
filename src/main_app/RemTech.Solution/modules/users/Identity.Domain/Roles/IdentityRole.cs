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

    private IdentityRole(RoleId id, RoleName name) => (Id, Name) = (id, name);

    public IdentityUserRoleEventArgs ToEventArgs() => new(Id.Value, Name.Value);

    public async Task<Status> PublishEvents(
        IDomainEventsDispatcher handler,
        CancellationToken ct = default
    ) => await handler.Dispatch(_events, ct);

    public static IdentityRole Create(RoleName name, RoleId? id = null)
    {
        RoleId creatingId = id ?? new RoleId();
        IdentityRole role = new(creatingId, name);
        if (id == null)
            role._events.Add(new RoleCreatedEvent(creatingId.Value, name.Value));
        return role;
    }
}
