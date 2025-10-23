using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Entities.Tickets.ValueObjects;

public sealed record UserTicketIssuerId
{
    public Guid Id { get; }

    public static Status<UserTicketIssuerId> Create(Guid id) =>
        id == Guid.Empty
            ? Error.Validation("Издатель токена не может быть без идентификатора.")
            : new UserTicketIssuerId(id);

    private UserTicketIssuerId(Guid id) => Id = id;
}
