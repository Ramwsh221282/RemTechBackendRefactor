namespace Tickets.Core;

public sealed class Ticket
{
    private readonly TicketLifecycle _lifeCycle;
    private readonly TicketMetadata _metadata;

    public Result<Ticket> ActivateBy(Guid creatorId)
    {
        if (!_metadata.IsCreatedBy(creatorId)) return Conflict("Заявка не принадлежит этому создателю.");
        Result<TicketLifecycle> activated = _lifeCycle.Activated();
        if (activated.IsFailure) return activated.Error;
        return new Ticket(activated, _metadata);
    }

    public Result<Ticket> MarkPending()
    {
        Result<TicketLifecycle> pending = _lifeCycle.Pending();
        if (pending.IsFailure) return pending.Error;
        return new Ticket(_lifeCycle, _metadata);
    }

    public TicketSnapshot Snapshot()
    {
        return new TicketSnapshot(_lifeCycle.Snapshot(), _metadata.Snapshot());
    }
    
    public static Result<Ticket> Create(Result<TicketMetadata> metadataResult, Result<TicketLifecycle> lifeCycleStatus)
    {
        if (metadataResult.IsFailure) return metadataResult.Error;
        if (lifeCycleStatus.IsFailure) return lifeCycleStatus.Error;
        return new Ticket(lifeCycleStatus, metadataResult);
    }
    
    private Ticket(TicketLifecycle lifeCycle, TicketMetadata metadata) =>
        (_lifeCycle, _metadata) = (lifeCycle, metadata);
}