using Identity.Domain.Roles.ValueObjects;

namespace Identity.Domain.Roles;

public sealed class Role
{
    public required RoleId Id { get; init; }
    public required RoleName Name { get; init; }
}
