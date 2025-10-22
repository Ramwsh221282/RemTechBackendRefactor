using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public interface IGetUserByIdHandle
{
    Task<Status<IdentityUser>> Handle(Guid userId, CancellationToken ct = default);
}
