using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public interface IGetUserByTicketHandle
{
    Task<Status<User>> Handle(Guid ticketId, CancellationToken ct = default);
}
