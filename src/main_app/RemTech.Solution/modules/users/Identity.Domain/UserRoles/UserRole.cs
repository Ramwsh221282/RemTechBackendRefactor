using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.UserRoles;

public sealed class UserRole
{
    public required UserId UserId { get; init; }
    public required RoleId RoleId { get; init; }
}
