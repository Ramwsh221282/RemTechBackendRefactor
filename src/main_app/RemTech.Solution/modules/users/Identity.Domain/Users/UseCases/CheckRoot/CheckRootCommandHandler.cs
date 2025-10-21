using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.CheckRoot;

public sealed class CheckRootCommandHandler(IUsersStorage users)
    : ICommandHandler<CheckRootCommand, Status<IEnumerable<IdentityUser>>>
{
    public async Task<Status<IEnumerable<IdentityUser>>> Handle(
        CheckRootCommand command,
        CancellationToken ct = default
    )
    {
        RoleName required = RoleName.Root;
        IEnumerable<IdentityUser> roots = await users.Get(required, ct);
        return roots.Any()
            ? Status<IEnumerable<IdentityUser>>.Success(roots)
            : Status<IEnumerable<IdentityUser>>.Failure(
                Error.NotFound("Root пользователи не найдены.")
            );
    }
}
