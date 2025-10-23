using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public interface IGetUserByIdHandle
{
    Task<Status<User>> Handle(Guid userId, CancellationToken ct = default);
}
