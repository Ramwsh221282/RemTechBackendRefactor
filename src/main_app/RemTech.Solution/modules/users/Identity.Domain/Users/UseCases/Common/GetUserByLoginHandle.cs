using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public sealed class GetUserByLoginHandle(IUsersStorage storage) : IGetUserByLoginHandle
{
    public async Task<Status<User>> Handle(string? login, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(login))
            return Error.NotFound("Пользователь не найден.");

        var userLogin = UserLogin.Create(login);
        if (userLogin.IsFailure)
            return Error.NotFound("Пользователь не найден.");

        var user = await storage.Get(userLogin, ct);
        return user == null ? Error.NotFound("Пользователь не найден.") : user;
    }
}
