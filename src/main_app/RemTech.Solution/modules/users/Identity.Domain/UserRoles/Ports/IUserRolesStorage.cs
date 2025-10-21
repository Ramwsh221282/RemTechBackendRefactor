using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.UserRoles.Ports;

public interface IUserRolesStorage
{
    Task Add(UserRole role, CancellationToken ct = default);
    Task<UserRole?> Get(UserId userId, RoleName roleName, CancellationToken ct = default);
    Task<IEnumerable<UserRole>> Get(IdentityUser identityUser, CancellationToken ct = default);
}
