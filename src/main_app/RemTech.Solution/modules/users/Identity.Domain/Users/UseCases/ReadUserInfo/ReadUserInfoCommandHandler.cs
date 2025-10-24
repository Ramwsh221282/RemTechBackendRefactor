using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.UseCases.Common;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.ReadUserInfo;

public sealed class ReadUserInfoCommandHandler(IGetUserByIdHandle handle)
    : ICommandHandler<ReadUserInfoCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        ReadUserInfoCommand command,
        CancellationToken ct = default
    )
    {
        if (command.Id == Guid.Empty || command.Id == null)
            return Error.NotFound("Пользователь не найден.");

        Status<User> user = await handle.Handle(command.Id, ct);
        return user.IsFailure ? user.Error : user;
    }
}
