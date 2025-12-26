using Identity.Domain.Roles.Enums;
using Identity.Domain.Roles.Events;
using Identity.Domain.Roles.ValueObjects;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Roles;

public sealed class IdentityRole
{
    private readonly List<IDomainEvent> _events = [];
    private IReadOnlyList<IdentityRole> CanBeChangedBy { get; set; } = [];

    public RoleId Id { get; }
    public RoleName Name { get; }

    public async Task<Status> PublishEvents(
        IDomainEventsDispatcher handler,
        CancellationToken ct = default
    ) => await handler.Dispatch(_events, ct);

    public Status UpdateChangers(IdentityRole role, RoleUpdateChangeOperationType type) =>
        type switch
        {
            RoleUpdateChangeOperationType.Add => AllowChangeFor(role),
            RoleUpdateChangeOperationType.Drop => DisallowChangeFor(role),
            _ => Status.Validation("Неподдерживаемая операция."),
        };

    private Status AllowChangeFor(IdentityRole role)
    {
        if (ContainsChanger(role))
            return Status.Conflict(
                $"Роль: {Name.Value} уже может быть изменена ролью: {role.Name.Value}, {role.Id.Value}"
            );

        CanBeChangedBy = WithChanger(role).ToList();
        RoleEventArgs target = ToEventArgs();
        IEnumerable<RoleEventArgs> changers = CanBeChangedBy.Select(c => c.ToEventArgs());
        _events.Add(new RoleChangeByOtherRolesUpdatedEvent(target, changers));
        return Status.Success();
    }

    private Status DisallowChangeFor(IdentityRole role)
    {
        if (!ContainsChanger(role))
            return Status.Conflict(
                $"Роль: {role.Name.Value} отсутствует в списке ролей, которые могут изменить текущую роль."
            );

        CanBeChangedBy = WithoutChanger(role).ToList();
        RoleEventArgs target = ToEventArgs();
        IEnumerable<RoleEventArgs> changers = CanBeChangedBy.Select(c => c.ToEventArgs());
        _events.Add(new RoleChangeByOtherRolesUpdatedEvent(target, changers));
        return Status.Success();
    }

    internal RoleEventArgs ToEventArgs() => new(Id.Value, Name.Value);

    private IEnumerable<IdentityRole> WithChanger(IdentityRole changer) =>
        [changer, .. CanBeChangedBy];

    private IEnumerable<IdentityRole> WithoutChanger(IdentityRole changer) =>
        [.. CanBeChangedBy.Where(c => c.Id != changer.Id)];

    private bool ContainsChanger(IdentityRole role) =>
        CanBeChangedBy.Any(r => r.Name == role.Name && r.Id == role.Id);

    private IdentityRole(RoleId id, RoleName name) => (Id, Name) = (id, name);

    public static IdentityRole Create(RoleName name, RoleId? id = null)
    {
        RoleId creatingId = id ?? new RoleId();
        IdentityRole role = new(creatingId, name);
        if (id == null)
            role._events.Add(new RoleCreatedEvent(role.ToEventArgs()));
        return role;
    }

    public static IdentityRole Create(RoleName name, RoleId id, IEnumerable<IdentityRole> changers)
    {
        IdentityRole created = Create(name, id);
        created.CanBeChangedBy = changers.ToList();
        return created;
    }

    public static Status<RoleUpdateChangeOperationType> UpdateChangersOperation(string name) =>
        !Enum.TryParse(name, true, out RoleUpdateChangeOperationType result)
            ? Error.Validation("Неподдерживаемая операция.")
            : result;
}
