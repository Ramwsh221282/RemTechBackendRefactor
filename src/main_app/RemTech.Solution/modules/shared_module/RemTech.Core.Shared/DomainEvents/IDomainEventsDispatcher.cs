using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.DomainEvents;

public interface IDomainEventsDispatcher
{
    Task<Status> Dispatch(IEnumerable<IDomainEvent> events, CancellationToken ct = default);
}
