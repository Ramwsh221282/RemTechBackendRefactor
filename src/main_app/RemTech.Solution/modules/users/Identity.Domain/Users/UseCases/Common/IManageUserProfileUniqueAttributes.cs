using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public interface IManageUserProfileUniqueAttributes
{
    Task<Status> HasUniqueProfileAttributes(User user, CancellationToken ct = default);
}
