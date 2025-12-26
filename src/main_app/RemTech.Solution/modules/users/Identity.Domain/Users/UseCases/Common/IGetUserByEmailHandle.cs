using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public interface IGetUserByEmailHandle
{
    Task<Status<User>> Handle(string? email, CancellationToken ct = default);
}
