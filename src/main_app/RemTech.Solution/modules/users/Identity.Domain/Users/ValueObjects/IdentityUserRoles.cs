using Identity.Domain.Roles;
using Identity.Domain.Roles.Events;
using Identity.Domain.Roles.ValueObjects;

namespace Identity.Domain.Users.ValueObjects;

public sealed record IdentityUserRoles(IEnumerable<IdentityRole> Roles)
{
    public bool HasRole(IdentityRole role) => HasRole(role.Name);

    public bool HasNotRole(IdentityRole role) => HasNotRole(role.Name);

    public static IEnumerable<RoleEventArgs> ToEventArgs(IdentityUserRoles roles) =>
        roles.Roles.Select(r => r.ToEventArgs());

    public IdentityUserRoles With(IdentityRole role) => new([role, .. Roles]);

    public IdentityUserRoles Without(IdentityRole role) =>
        new(Roles.Where(r => r.Name != role.Name));

    private bool HasRole(RoleName name) => Roles.Any(r => r.Name == name);

    private bool HasNotRole(RoleName name) => !Roles.Any(r => r.Name == name);
}
