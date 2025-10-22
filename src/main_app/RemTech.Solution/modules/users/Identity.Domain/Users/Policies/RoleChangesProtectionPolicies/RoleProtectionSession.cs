using Identity.Domain.Roles;
using Identity.Domain.Roles.ValueObjects;

namespace Identity.Domain.Users.Policies.RoleChangesProtectionPolicies;

public sealed record RoleProtectionSession(RoleName Existing, RoleName Actor)
{
    public RoleProtectionSession(IdentityRole existing, IdentityRole actor)
        : this(existing.Name, actor.Name) { }
}
