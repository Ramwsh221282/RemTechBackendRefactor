using Dapper.FluentMap.Mapping;

namespace Identity.Adapter.Storage.Storages.Models;

internal sealed class UserData
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required bool EmailConfirmed { get; init; }
    public required string Password { get; init; }
    public required List<RoleData> Roles { get; init; } = [];
}

internal sealed class UserDataMap : EntityMap<UserData>
{
    public UserDataMap()
    {
        Map(u => u.Id).ToColumn("user_id");
        Map(u => u.Name).ToColumn("user_login");
        Map(u => u.Email).ToColumn("user_email");
        Map(u => u.EmailConfirmed).ToColumn("user_email_confirmed");
        Map(u => u.Password).ToColumn("user_password");
        Map(u => u.Roles).Ignore();
    }
}
