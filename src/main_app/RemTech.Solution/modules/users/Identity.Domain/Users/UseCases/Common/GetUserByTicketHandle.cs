using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities.Tickets.ValueObjects;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public sealed class GetUserByTicketHandle(IUsersStorage storage) : IGetUserByTicketHandle
{
    public async Task<Status<User>> Handle(Guid ticketId, CancellationToken ct = default)
    {
        var id = UserTicketId.Create(ticketId);
        var user = await storage.Get(id, ct);
        return user == null ? Error.NotFound("Пользователь по заявке не был найден.") : user;
    }
}