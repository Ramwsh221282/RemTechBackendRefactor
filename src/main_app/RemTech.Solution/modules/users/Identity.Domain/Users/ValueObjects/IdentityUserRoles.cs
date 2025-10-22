using Identity.Domain.Roles;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Events;

namespace Identity.Domain.Users.ValueObjects;

public sealed record IdentityUserRoles(IEnumerable<IdentityRole> Roles)
{
    public bool HasRole(RoleName name) => Roles.Any(r => r.Name == name);

    public bool HasNotRole(RoleName name) => !Roles.Any(r => r.Name == name);

    public static IEnumerable<IdentityUserRoleEventArgs> ToEventArgs(IdentityUserRoles roles)
    {
        return roles.Roles.Select(r => new IdentityUserRoleEventArgs(r.Id.Value, r.Name.Value));
    }
}
