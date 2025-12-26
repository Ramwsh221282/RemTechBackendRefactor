using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.ReadUsers;

public sealed class ReadUsersCommandHandler(IUsersStorage users)
    : ICommandHandler<ReadUsersCommand, IEnumerable<User>>
{
    public async Task<IEnumerable<User>> Handle(
        ReadUsersCommand command,
        CancellationToken ct = default
    )
    {
        var specification = command.Specification;
        var data = await users.Get(specification, ct);
        return data;
    }
}
