using Identity.Domain.Users.Events;
using RemTech.Core.Shared.Result;

namespace Identity.Adapter.Storage.EventHandlers;

public interface IIdentityStorageAdapterEventHandler<TEvent>
    where TEvent : IdentityUserEvent
{
    Task<Status> Handle(TEvent @event, CancellationToken ct = default);
}
