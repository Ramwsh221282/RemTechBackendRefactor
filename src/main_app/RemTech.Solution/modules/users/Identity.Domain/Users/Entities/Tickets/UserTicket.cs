using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities.Tickets.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Entities.Tickets;

public sealed class UserTicket
{
    public UserTicketId Id { get; }
    public UserTicketIssuerId IssuerId { get; }
    public UserTicketType Type { get; }
    public UserTicketLifeTime LifeTime { get; private set; }

    public UserTicket(
        UserTicketIssuerId issuerId,
        UserTicketType type,
        UserTicketLifeTime lifeTime,
        UserTicketId? id = null
    ) => (Id, IssuerId, Type, LifeTime) = (id ?? new UserTicketId(), issuerId, type, lifeTime);

    public Status Confirm(User user)
    {
        if (!BelongsTo(user))
            return Status.Conflict("Данный токен не принадлежит этому пользователю.");
        if (!IsAlive())
            return Status.Conflict("Сессия токена была завершена.");

        var deleted = DateTime.UtcNow;
        var newLifeTime = UserTicketLifeTime.Create(LifeTime.Created, LifeTime.Expires, deleted);
        if (newLifeTime.IsFailure)
            return Status.Conflict(newLifeTime.Error.ErrorText);

        LifeTime = newLifeTime.Value;
        return Status.Success();
    }

    public bool BelongsTo(User user) => IssuerId.Id == user.Id.Id;

    public bool IsAlive() => LifeTime.IsAlive();
}
