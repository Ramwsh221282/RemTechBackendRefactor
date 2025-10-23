using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public interface IGetVerifiedUserHandle
{
    Task<Status<User>> Handle(Guid userId, string userPassword, CancellationToken ct = default);
}
