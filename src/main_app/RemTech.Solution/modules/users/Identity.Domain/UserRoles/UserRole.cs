using Identity.Domain.Roles;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users;
using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.UserRoles;

public sealed class UserRole
{
    public UserId UserId { get; }
    public RoleId RoleId { get; }

    public UserRole(UserId userId, RoleId roleId) => (UserId, RoleId) = (userId, roleId);

    public UserRole(User user, Role role)
        : this(user.Id, role.Id) { }
}
