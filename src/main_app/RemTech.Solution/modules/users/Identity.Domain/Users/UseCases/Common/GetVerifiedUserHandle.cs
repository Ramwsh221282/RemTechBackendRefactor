using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public sealed class GetVerifiedUserHandle(IUsersStorage users, IStringHashAlgorithm manager)
    : IGetVerifiedUserHandle
{
    public async Task<Status<User>> Handle(
        Guid userId,
        string userPassword,
        CancellationToken ct = default
    )
    {
        UserId id = UserId.Create(userId);
        User? user = await users.Get(id, ct);
        if (user == null)
            return Error.NotFound("Пользователь не найден.");

        UserPassword password = UserPassword.Create(userPassword);
        Status verification = user.Verify(password, manager);
        if (verification.IsFailure)
            return verification.Error;

        return user;
    }
}
