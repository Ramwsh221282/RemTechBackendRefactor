using RemTech.Outbox.Shared;

namespace Tickets.Outbox;

public interface ITicketsOutboxJobMethod
{
    Task<ProcessedOutboxMessages> ProcessMessages(CancellationToken ct);
}