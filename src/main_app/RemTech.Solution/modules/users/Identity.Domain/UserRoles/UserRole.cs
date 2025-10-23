using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate.ValueObjects;

namespace Identity.Domain.UserRoles;

public sealed class UserRole
{
    public UserId UserId { get; }
    public RoleId RoleId { get; }

    public UserRole(UserId userId, RoleId roleId) => (UserId, RoleId) = (userId, roleId);
}
