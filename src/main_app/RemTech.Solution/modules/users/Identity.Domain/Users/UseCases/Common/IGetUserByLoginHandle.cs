using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public interface IGetUserByLoginHandle
{
    Task<Status<User>> Handle(string? login, CancellationToken ct = default);
}
