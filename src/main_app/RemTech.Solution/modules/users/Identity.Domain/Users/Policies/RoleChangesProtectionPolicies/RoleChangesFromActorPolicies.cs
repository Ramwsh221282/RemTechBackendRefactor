using Identity.Domain.Roles;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;

namespace Identity.Domain.Users.Policies.RoleChangesProtectionPolicies;

public static class RoleChangesFromActorPolicies
{
    private static readonly HashSet<RoleName> Changers = [RoleName.Root];

    public static bool IsRoleChangeAllowed(this IdentityUser user)
    {
        bool allowedAny = false;
        foreach (IdentityRole role in user.Roles.Roles)
        {
            RoleName name = role.Name;
            if (!Changers.Add(name))
            {
                allowedAny = true;
                break;
            }
        }

        return allowedAny;
    }
}
