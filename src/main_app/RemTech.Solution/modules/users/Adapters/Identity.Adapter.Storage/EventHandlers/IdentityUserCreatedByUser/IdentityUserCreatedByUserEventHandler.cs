using Identity.Adapter.Storage.EventHandlers.IdentityUserCreated;
using Identity.Domain.Users.Events;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Adapter.Storage.EventHandlers.IdentityUserCreatedByUser;

public sealed class IdentityUserCreatedByUserEventHandler(IdentityDbContext context)
    : IDomainEventHandler<IdentityUserCreatedByUserEvent>
{
    public async Task<Status> Handle(
        IdentityUserCreatedByUserEvent @event,
        CancellationToken ct = default
    )
    {
        return await new IdentityUserCreatedEventHandler(context).Handle(@event.CreatedInfo, ct);
    }
}
