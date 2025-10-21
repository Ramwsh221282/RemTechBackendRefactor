using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.CheckRoot;

public sealed class CheckRootCommandHandler(IUsersStorage users)
    : ICommandHandler<CheckRootCommand, Status<IEnumerable<User>>>
{
    public async Task<Status<IEnumerable<User>>> Handle(
        CheckRootCommand command,
        CancellationToken ct = default
    )
    {
        RoleName required = RoleName.Root;
        IEnumerable<User> roots = await users.Get(required, ct);
        return roots.Any()
            ? Status<IEnumerable<User>>.Success(roots)
            : Status<IEnumerable<User>>.Failure(Error.NotFound("Root пользователи не найдены."));
    }
}
