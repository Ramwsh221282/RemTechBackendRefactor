using Identity.Domain.Users.Entities;
using Identity.Domain.Users.ValueObjects;
using IdentityUserToken = Identity.Domain.Users.Entities.IdentityUserToken;

namespace Identity.Domain.Users.Events;

public sealed record EmailConfirmationCreatedTokenEvent(
    Guid UserId,
    Guid TokenId,
    DateTime Created,
    DateTime Expires,
    string TokenType,
    string UserEmail
) : IdentityUserEvent
{
    public EmailConfirmationCreatedTokenEvent(
        IdentityUserToken token,
        IdentityUserProfile profile,
        UserId id
    )
        : this(id.Id, token.Id.Id, token.Created, token.Expires, token.Type, profile.Email.Email)
    { }

    public EmailConfirmationCreatedTokenEvent(
        UserId id,
        IdentityUserProfile profile,
        Guid tokenId,
        DateTime created,
        DateTime expires,
        string tokenType
    )
        : this(id.Id, tokenId, created, expires, tokenType, profile.Email.Email) { }
}
