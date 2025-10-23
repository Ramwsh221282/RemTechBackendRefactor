using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record UserCreatedByUser(UserCreated CreatorInfo, UserCreated CreatedInfo)
    : IDomainEvent;
