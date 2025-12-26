using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Entities.Tickets.ValueObjects;

public sealed record UserTicketId
{
    public Guid Id { get; }

    private UserTicketId(Guid id) => Id = id;

    public UserTicketId() => Id = Guid.NewGuid();

    public static Status<UserTicketId> Create(Guid id) =>
        id == Guid.Empty
            ? Error.Validation("Идентификатор тикета пользователя был пустым.")
            : new UserTicketId(id);
}
