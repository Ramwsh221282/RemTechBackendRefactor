using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public sealed class GetUserByIdHandle(IUsersStorage users) : IGetUserByIdHandle
{
    public async Task<Status<User>> Handle(Guid userId, CancellationToken ct = default)
    {
        UserId id = UserId.Create(userId);
        User? user = await users.Get(id, ct);
        return user == null ? Error.NotFound("Пользователь не найден.") : user;
    }
}
