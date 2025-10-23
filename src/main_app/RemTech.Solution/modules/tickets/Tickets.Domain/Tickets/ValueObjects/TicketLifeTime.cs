using RemTech.Core.Shared.Result;

namespace Tickets.Domain.Tickets.ValueObjects;

public readonly record struct TicketLifeTime
{
    public DateTime Created { get; }
    public DateTime? Deleted { get; }

    public TicketLifeTime()
    {
        Created = DateTime.UtcNow;
        Deleted = null;
    }

    private TicketLifeTime(DateTime created, DateTime? deleted)
    {
        Created = created;
        Deleted = deleted;
    }

    public static Status<TicketLifeTime> Create(DateTime created, DateTime? deleted)
    {
        if (created == DateTime.MinValue || created == DateTime.MaxValue)
            return Error.Validation("Некорректная дата создания заявки.");

        if (deleted != null)
            if (created > deleted)
                return Error.Validation("Дата удаления заявки не может быть ранее даты создания.");

        return new TicketLifeTime(created, deleted);
    }
}
