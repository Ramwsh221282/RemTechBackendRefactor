using Identity.Domain.Roles;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;

namespace Identity.Domain.Users.Policies.RoleChangesProtectionPolicies;

public static class RoleChangesToTargetPolicies
{
    private static readonly HashSet<RoleProtectionSession> TargetCanBeChangedBy =
    [
        new(RoleName.Admin, RoleName.Root),
        new(RoleName.User, RoleName.Root),
    ];

    public static bool CanBeChangedBy(this User target, User actor)
    {
        bool allowedAny = false;
        foreach (IdentityRole targetRole in target.Roles.Roles)
        {
            foreach (IdentityRole actorRole in actor.Roles.Roles)
            {
                RoleProtectionSession session = new(targetRole, actorRole);
                if (!TargetCanBeChangedBy.Add(session))
                {
                    allowedAny = true;
                    break;
                }
            }
        }

        return allowedAny;
    }
}
