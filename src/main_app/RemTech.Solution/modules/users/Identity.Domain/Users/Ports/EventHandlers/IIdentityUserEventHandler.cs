using Identity.Domain.Users.Events;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Ports.EventHandlers;

public interface IIdentityUserEventHandler
{
    Task<Status> Handle(IEnumerable<IdentityUserEvent> events, CancellationToken ct = default);
}
