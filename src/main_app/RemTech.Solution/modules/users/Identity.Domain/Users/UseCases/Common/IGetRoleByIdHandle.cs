using Identity.Domain.Roles;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public interface IGetRoleByIdHandle
{
    Task<Status<IdentityRole>> Handle(string name, CancellationToken ct = default);
}
