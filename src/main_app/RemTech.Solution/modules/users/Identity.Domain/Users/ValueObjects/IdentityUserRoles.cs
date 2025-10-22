using Identity.Domain.Roles;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Events;

namespace Identity.Domain.Users.ValueObjects;

public sealed record IdentityUserRoles(IEnumerable<IdentityRole> Roles)
{
    public bool HasRole(IdentityRole role) => HasRole(role.Name);

    public bool HasNotRole(IdentityRole role) => HasNotRole(role.Name);

    public static IEnumerable<IdentityUserRoleEventArgs> ToEventArgs(IdentityUserRoles roles) =>
        roles.Roles.Select(r => new IdentityUserRoleEventArgs(r.Id.Value, r.Name.Value));

    public IdentityUserRoles With(IdentityRole role) => new([role, .. Roles]);

    public IdentityUserRoles Without(IdentityRole role) =>
        new(Roles.Where(r => r.Name != role.Name));

    private bool HasRole(RoleName name) => Roles.Any(r => r.Name == name);

    private bool HasNotRole(RoleName name) => !Roles.Any(r => r.Name == name);
}
