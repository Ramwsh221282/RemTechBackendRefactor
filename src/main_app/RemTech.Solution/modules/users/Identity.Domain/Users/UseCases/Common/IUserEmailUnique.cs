using Identity.Domain.Users.Entities.Profile.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public interface IUserEmailUnique
{
    Task<Status> Unique(UserEmail email, CancellationToken ct = default);
}