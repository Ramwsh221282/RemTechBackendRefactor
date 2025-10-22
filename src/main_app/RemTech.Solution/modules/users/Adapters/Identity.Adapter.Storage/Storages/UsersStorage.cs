using System.Data;
using Dapper;
using Identity.Adapter.Storage.Storages.Extensions;
using Identity.Adapter.Storage.Storages.Models;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using Shared.Infrastructure.Module.Postgres;

namespace Identity.Adapter.Storage.Storages;

public sealed class UsersStorage(PostgresDatabase database) : IUsersStorage
{
    public async Task<IdentityUser?> Get(UserEmail email, CancellationToken ct = default) =>
        await QueryUser(["u.email = @email"], new { email = email.Email }, ct);

    public async Task<IdentityUser?> Get(UserLogin login, CancellationToken ct = default) =>
        await QueryUser(["u.name = @name"], new { name = login.Name }, ct);

    public async Task<IdentityUser?> Get(UserId id, CancellationToken ct = default) =>
        await QueryUser(["u.id = @id"], new { id = id.Id }, ct);

    public async Task<IdentityUser?> Get(IdentityTokenId id, CancellationToken ct = default) =>
        null;

    public async Task<IEnumerable<IdentityUser>> Get(
        RoleName role,
        CancellationToken ct = default
    ) => await QueryUsers(["r.name = @name"], new { name = role.Value }, ct);

    private static string CreateSql(List<string> whereClauses)
    {
        string whereClause =
            whereClauses.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", whereClauses);

        return $"""
            SELECT
            u.id as user_id,
            u.name as user_login,
            u.email as user_email,
            u.email_confirmed as user_email_confirmed,
            u.password as user_password,
            r.id as role_id,
            r.name as role_name
            FROM users_module.users u
            LEFT JOIN users_module.user_roles ur ON u.id = ur.user_id
            LEFT JOIN users_module.roles r ON r.id = ur.role_id
            {whereClause}
            """;
    }

    private static UserData MapUser(Dictionary<Guid, UserData> users, UserData user, RoleData role)
    {
        if (!users.TryGetValue(user.Id, out UserData? userData))
        {
            userData = user;
            users.Add(user.Id, userData);
        }

        userData.Roles.Add(role);
        return userData;
    }

    private async Task<IEnumerable<IdentityUser>> QueryUsers(
        List<string> whereClauses,
        object? parameters = null,
        CancellationToken ct = default
    )
    {
        using IDbConnection connection = await database.ProvideConnection(ct: ct);
        CommandDefinition command = new(CreateSql(whereClauses), parameters, cancellationToken: ct);
        Dictionary<Guid, UserData> users = [];

        IEnumerable<UserData> data = await connection.QueryAsync<UserData, RoleData, UserData>(
            command,
            (user, role) => MapUser(users, user, role),
            splitOn: "role_id"
        );

        return data.Select(d => d.ToIdentityUser());
    }

    private async Task<IdentityUser?> QueryUser(
        List<string> whereClauses,
        object? parameters = null,
        CancellationToken ct = default
    )
    {
        IEnumerable<IdentityUser> users = await QueryUsers(whereClauses, parameters, ct);
        return users.SingleOrDefault();
    }
}
