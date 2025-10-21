namespace Identity.Domain.Users.Events;

public sealed record IdentityUserCreatedByUserEvent(
    IdentityUserCreatedEvent CreatorInfo,
    IdentityUserCreatedEvent CreatedInfo
) : IdentityUserEvent;
