using RemTech.Core.Shared.Result;

namespace Identity.Domain.Roles.ValueObjects;

public readonly record struct RoleId
{
    public Guid Value { get; }

    public RoleId() => Value = Guid.NewGuid();

    private RoleId(Guid value) => Value = value;

    public static Status<RoleId> Create(Guid id) =>
        id == Guid.Empty
            ? Error.Validation("Идентификатор роли не может быть пустым.")
            : new RoleId(id);
}
