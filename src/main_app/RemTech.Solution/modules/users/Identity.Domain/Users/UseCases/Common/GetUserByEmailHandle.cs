using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public sealed class GetUserByEmailHandle(IUsersStorage storage) : IGetUserByEmailHandle
{
    public async Task<Status<User>> Handle(string? email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Error.NotFound("пользователь не найден.");

        var userEmail = UserEmail.Create(email);
        if (userEmail.IsFailure)
            return Error.NotFound("пользователь не найден.");

        User? required = await storage.Get(userEmail, ct);
        return required == null ? Error.NotFound("пользователь не найден.") : required;
    }
}
