using Identity.Domain.Roles.ValueObjects;

namespace Identity.Domain.Roles.Ports;

public interface IRolesStorage
{
    Task<IdentityRole?> Get(RoleName name, CancellationToken ct = default);
    Task<IdentityRole?> Get(RoleId id, CancellationToken ct = default);
}
