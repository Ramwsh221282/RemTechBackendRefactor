using Identity.Domain.Users.Entities;
using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.Users.Events;

public sealed record IdentityUserRemovedEventInfo(
    Guid UserId,
    string UserLogin,
    string UserEmail,
    string UserPassword,
    IEnumerable<string> RoleNames
)
{
    public IdentityUserRemovedEventInfo(
        UserId id,
        IdentityUserProfile profile,
        IdentityUserRoles roles
    )
        : this(
            id.Id,
            profile.Login.Name,
            profile.Email.Email,
            profile.Password.Password,
            roles.Roles.Select(r => r.Name.Value)
        ) { }
}

public sealed record IdentityUserRemovedEvent(
    IdentityUserRemovedEventInfo RemoverInfo,
    IdentityUserRemovedEventInfo RemovedInfo
) : IdentityUserEvent;
