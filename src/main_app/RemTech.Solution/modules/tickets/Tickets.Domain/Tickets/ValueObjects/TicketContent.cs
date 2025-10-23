using RemTech.Core.Shared.Result;

namespace Tickets.Domain.Tickets.ValueObjects;

public sealed record TicketContent
{
    public string Value { get; }

    private TicketContent(string value) => Value = value;

    public static Status<TicketContent> Create(string content) =>
        string.IsNullOrWhiteSpace(content)
            ? Error.Validation("Содержимое заявки было пустым.")
            : new TicketContent(content);
}
