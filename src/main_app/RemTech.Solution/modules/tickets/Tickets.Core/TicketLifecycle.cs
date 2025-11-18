using RemTech.Primitives.Extensions.SmartEnumerations;
using Tickets.Core.Snapshots;

namespace Tickets.Core;

public sealed class TicketLifecycle
{
    private readonly Guid _ticketId;
    private readonly DateTime _created;
    private readonly Optional<DateTime> _closed;
    private readonly TicketStatus _status;
    
    public Result<TicketLifecycle> Activated()
    {
        Result<TicketStatus> status = _status.Change(new TicketStatus.Activated());
        if (status.IsFailure) return status.Error;
        Optional<DateTime> closed = DateTime.UtcNow;
        return new TicketLifecycle(_ticketId, _created, closed, status);
    }

    public Result<TicketLifecycle> Pending()
    {
        Result<TicketStatus> status = _status.Change(new TicketStatus.Pending());
        if (status.IsFailure) return status.Error;
        return new TicketLifecycle(_ticketId, _created, _closed, status);
    }

    public TicketLifeCycleSnapshot Snapshot()
    {
        return new TicketLifeCycleSnapshot(_ticketId, _created, _closed.HasValue ? _closed.Value : null, _status.Name);
    }
    
    private TicketLifecycle(Guid ticketId, DateTime created, DateTime? closed, TicketStatus status)
        : this(ticketId, created, closed.HasValue ? Some(closed.Value) : None<DateTime>(), status) { }
    
    private TicketLifecycle(Guid ticketId, DateTime created, Optional<DateTime> closed, TicketStatus status) =>
        (_ticketId, _created, _closed, _status) = (ticketId, created, closed, status);
    
    internal static Result<TicketLifecycle> Create(Guid ticketId, DateTime created, DateTime? closed, TicketStatus status)
    {
        return Create(ticketId, created, closed, status.Name);
    }
    
    public static Result<TicketLifecycle> Create(Guid ticketId, DateTime created, DateTime? closed, string status)
    {
        if (ticketId == Guid.Empty) return Validation("Идентификатор заявки пустой.");
        if (created == DateTime.MinValue || created == DateTime.MaxValue) return Validation("Дата создания заявки невалидная.");
        if (closed.HasValue)
        {
            if (closed.Value == DateTime.MinValue || closed.Value == DateTime.MinValue)
                return Validation("Дата окончания заявки невалидная.");
            if (closed.Value < created)
                return Validation("Дата окончания заявки невалидная.");
        }
        if (!SmartEnumerations.Exists(status, (s, en) => s == en.Name, out TicketStatus result))
            return Validation($"Статус заявки не поддерживается: {status}");

        return new TicketLifecycle(ticketId, created, closed.HasValue ? Some(closed.Value) : None<DateTime>(), result);
    }
}