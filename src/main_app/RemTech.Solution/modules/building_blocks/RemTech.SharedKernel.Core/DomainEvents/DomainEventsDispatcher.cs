using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace RemTech.SharedKernel.Core.DomainEvents;

public sealed class DomainEventsDispatcher(IEnumerable<IDomainEventHandler> handlers)
{
    public async Task Dispatch(IDomainEventBearer bearer, CancellationToken ct = default)
    {
        await Dispatch(bearer.Events, ct);
    }

    public async Task Dispatch(IDomainEvent @event, CancellationToken ct = default)
    {
        foreach (IDomainEventHandler handler in handlers)
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
