using RemTech.Core.Shared.Result;

namespace Tickets.Domain.Tickets.ValueObjects;

public readonly record struct TicketId
{
    public Guid Value { get; }

    public TicketId() => Value = Guid.NewGuid();

    private TicketId(Guid value) => Value = value;

    public static Status<TicketId> Create(Guid value) =>
        value == Guid.Empty
            ? Error.Validation("Идентификатор заявки пустой.")
            : new TicketId(value);
}
