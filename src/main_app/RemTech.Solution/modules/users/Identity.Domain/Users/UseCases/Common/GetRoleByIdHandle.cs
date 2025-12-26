using Identity.Domain.Roles;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public sealed class GetRoleByIdHandle(IRolesStorage storage) : IGetRoleByIdHandle
{
    public async Task<Status<IdentityRole>> Handle(string name, CancellationToken ct = default)
    {
        RoleName roleName = RoleName.Create(name);
        IdentityRole? role = await storage.Get(roleName, ct);
        return role == null
            ? Error.NotFound($"Роль: {name} не существует.")
            : new Status<IdentityRole>(role);
    }
}
