using Identity.Domain.Roles.ValueObjects;

namespace Identity.Domain.Roles.Ports;

public interface IRolesStorage
{
    Task<Role?> Get(RoleName name, CancellationToken ct = default);
    Task<Role?> Get(RoleId id, CancellationToken ct = default);
}
