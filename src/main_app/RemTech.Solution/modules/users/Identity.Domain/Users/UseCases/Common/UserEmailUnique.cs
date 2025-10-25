using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public sealed class UserEmailUnique(IUsersStorage storage) : IUserEmailUnique
{
    public async Task<Status> Unique(UserEmail email, CancellationToken ct = default)
    {
        var user = await storage.Get(email, ct);
        return user is not null
            ? Status.Conflict($"Пользователь с почтой: {email.Email} уже существует.")
            : Status.Success();
    }
}