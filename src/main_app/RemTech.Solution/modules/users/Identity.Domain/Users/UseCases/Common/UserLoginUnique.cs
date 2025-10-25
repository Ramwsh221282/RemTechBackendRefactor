using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public sealed class UserLoginUnique(IUsersStorage storage) : IUserLoginUnique
{
    public async Task<Status> Unique(UserLogin login, CancellationToken ct = default)
    {
        var user = await storage.Get(login, ct);
        return user is not null
            ? Status.Conflict($"Пользователь с логином: {login.Name} уже существует.")
            : Status.Success();
    }
}