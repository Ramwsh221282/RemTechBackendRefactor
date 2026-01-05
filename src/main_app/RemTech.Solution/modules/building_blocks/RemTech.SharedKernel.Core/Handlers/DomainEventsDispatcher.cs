namespace RemTech.SharedKernel.Core.Handlers;

public sealed class DomainEventsDispatcher(IEnumerable<IDomainEventHandler> handlers)
{
    private IDomainEventHandler[] Handlers { get; } = handlers.ToArray();

    public async Task Dispatch(IDomainEvent @event, CancellationToken ct = default)
    {
        foreach (IDomainEventHandler handler in Handlers)
        {
            await handler.Handle(@event, ct);
        }
    }

    public async Task Dispatch(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
    {
        foreach (IDomainEvent @event in events)
        {
            await Dispatch(@event, ct);
        }
    }
}