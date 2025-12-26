using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public interface IGetUserHandle
{
    public Task<Status<User>> Get(string? login, string? email, CancellationToken ct = default);
}
