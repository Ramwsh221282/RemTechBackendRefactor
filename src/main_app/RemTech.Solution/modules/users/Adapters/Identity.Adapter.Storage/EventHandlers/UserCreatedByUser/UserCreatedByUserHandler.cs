using Identity.Adapter.Storage.EventHandlers.UserCreated;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Adapter.Storage.EventHandlers.UserCreatedByUser;

public sealed class UserCreatedByUserHandler(Serilog.ILogger logger, IdentityDbContext context)
    : IDomainEventHandler<Domain.Users.Events.UserCreatedByUser>
{
    public async Task<Status> Handle(
        Domain.Users.Events.UserCreatedByUser @event,
        CancellationToken ct = default
    ) => await new UserCreatedEventHandler(logger, context).Handle(@event.CreatedInfo, ct);
}
