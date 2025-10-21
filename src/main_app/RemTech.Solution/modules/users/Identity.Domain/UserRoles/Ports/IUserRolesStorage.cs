using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.UserRoles.Ports;

public interface IUserRolesStorage
{
    Task Add(UserRole role, CancellationToken ct = default);
    Task<UserRole?> Get(UserId userId, RoleName roleName, CancellationToken ct = default);
}
