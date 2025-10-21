using Identity.Domain.Users.Entities;
using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.Users.Events;

public sealed record IdentityUserCreatedEvent(
    Guid UserId,
    string UserLogin,
    string UserEmail,
    string UserPassword,
    Guid SessionId,
    string SessionRefreshToken,
    string SessionToken,
    bool SessionExpired,
    IEnumerable<string> RoleNames
) : IdentityUserEvent
{
    public IdentityUserCreatedEvent(
        UserId id,
        IdentityUserProfile profile,
        IdentityUserSession session,
        IdentityUserRoles roles
    )
        : this(
            id.Id,
            profile.Login.Name,
            profile.Email.Email,
            profile.Password.Password,
            session.SessionId,
            session.SessionRefreshToken,
            session.SessionToken,
            session.IsExpired,
            roles.Roles.Select(r => r.Name.Value)
        ) { }
}
