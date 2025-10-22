using Identity.Adapter.Storage.Storages.Models;
using Identity.Domain.Roles;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities;
using Identity.Domain.Users.ValueObjects;

namespace Identity.Adapter.Storage.Storages.Extensions;

internal static class UserDataExtensions
{
    internal static IdentityUser ToIdentityUser(this UserData data)
    {
        UserId userId = UserId.Create(data.Id);
        UserLogin login = UserLogin.Create(data.Name);
        UserEmail userEmail = UserEmail.Create(data.Email);
        bool emailConfirmed = data.EmailConfirmed;
        HashedUserPassword password = HashedUserPassword.Create(data.Password);
        IdentityUserProfile profile = new(login, userEmail, password, emailConfirmed);

        IEnumerable<IdentityRole> roles = data.Roles.Select(d => d.ToIdentityRole());
        IdentityUserRoles userRoles = new(roles);
        return IdentityUser.Create(profile, userRoles, userId);
    }

    internal static IdentityRole ToIdentityRole(this RoleData role)
    {
        RoleId id = RoleId.Create(role.Id);
        RoleName name = RoleName.Create(role.Name);
        return IdentityRole.Create(name, id);
    }
}
