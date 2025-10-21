using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.Users.Entities;

public sealed class IdentityUserToken
{
    public UserId UserId { get; }
    public IdentityTokenId Id { get; }
    public DateTime Created { get; }
    public DateTime Expires { get; }
    public string Type { get; }

    public IdentityUserToken(
        IdentityTokenId id,
        UserId userId,
        DateTime created,
        DateTime expires,
        string type
    ) => (Id, UserId, Created, Expires, Type) = (id, userId, created, expires, type);

    public bool BelongsToUser(UserId id) => UserId == id;

    public bool HasExpired() => Expires > DateTime.Now;

    public bool IsOfType(string type) => Type == type;
}
