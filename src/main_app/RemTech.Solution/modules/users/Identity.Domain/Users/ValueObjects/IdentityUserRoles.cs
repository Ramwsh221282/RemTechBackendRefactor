using Identity.Domain.Roles;
using Identity.Domain.Roles.ValueObjects;

namespace Identity.Domain.Users.ValueObjects;

public sealed record IdentityUserRoles(IEnumerable<Role> Roles)
{
    public bool HasRole(RoleName name) => Roles.Any(r => r.Name == name);

    public bool HasNotRole(RoleName name) => !Roles.Any(r => r.Name == name);
}
