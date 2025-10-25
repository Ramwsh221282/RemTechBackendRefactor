using Identity.Domain.Users.Entities.Profile.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public interface IUserLoginUnique
{
    Task<Status> Unique(UserLogin login, CancellationToken ct = default);
}