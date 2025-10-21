using Dapper;
using Identity.Domain.Roles;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using Npgsql;
using Shared.Infrastructure.Module.Postgres;

namespace Identity.Adapter.Storage.Storages;

internal record UserData(
    Guid UserId,
    string UserName,
    string UserEmail,
    bool EmailConfirmed,
    string Password,
    Guid? RoleId,
    string? RoleName
);

internal static class UserDataExtensions
{
    internal static IdentityUser ToIdentityUser(this IEnumerable<UserData> rows)
    {
        UserData firstRow = rows.First();
        UserId id = UserId.Create(firstRow.UserId);
        UserLogin login = UserLogin.Create(firstRow.UserName);
        UserEmail userEmail = UserEmail.Create(firstRow.UserEmail);
        bool emailConfirmed = firstRow.EmailConfirmed;
        HashedUserPassword password = HashedUserPassword.Create(firstRow.Password);
        IdentityUserProfile profile = new(login, userEmail, password, emailConfirmed);

        IEnumerable<Role> userRoles = rows.Where(r => r.RoleId != null && r.RoleName != null)
            .Select(row => new Role(
                RoleId.Create(row.RoleId!.Value),
                RoleName.Create(row.RoleName!)
            ));

        IdentityUserRoles roles = new(userRoles);
        return IdentityUser.Create(profile, roles, id);
    }
}

public sealed partial class UsersStorage(PostgresDatabase database) : IUsersStorage
{
    public async Task<IdentityUser?> Get(UserEmail email, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
            u.id as user_id,
            u.name as user_name,
            u.email as user_email,
            u.email_confirmed as user_email_confirmed,
            u.password as user_password,
            r.id as user_role_id,
            r.name as user_role_name
            FROM users_module.users u
            LEFT JOIN users_module.user_roles ur ON u.id = ur.user_id
            LEFT JOIN users_module.roles r ON r.id = ur.role_id
            WHERE u.email = @email
            FOR UPDATE
            """;

        CommandDefinition command = new(sql, new { email = email.Email }, cancellationToken: ct);
        await using NpgsqlConnection connection = await database.DataSource.OpenConnectionAsync(ct);
        IEnumerable<UserData> rows = await connection.QueryAsync<UserData>(command);
        return !rows.Any() ? null : rows.ToIdentityUser();
    }

    public async Task<IdentityUser?> Get(UserLogin login, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
            u.id as user_id,
            u.name as user_name,
            u.email as user_email,
            u.email_confirmed as user_email_confirmed,
            u.password as user_password,
            r.id as user_role_id,
            r.name as user_role_name
            FROM users_module.users u
            LEFT JOIN users_module.user_roles ur ON u.id = ur.user_id
            LEFT JOIN users_module.roles r ON r.id = ur.role_id
            WHERE u.name = @name
            FOR UPDATE
            """;

        CommandDefinition command = new(sql, new { name = login.Name }, cancellationToken: ct);
        await using NpgsqlConnection connection = await database.DataSource.OpenConnectionAsync(ct);
        IEnumerable<UserData> rows = await connection.QueryAsync<UserData>(command);
        return !rows.Any() ? null : rows.ToIdentityUser();
    }

    public async Task<IdentityUser?> Get(UserId id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
            u.id as user_id,
            u.name as user_name,
            u.email as user_email,
            u.email_confirmed as user_email_confirmed,
            u.password as user_password,
            r.id as user_role_id,
            r.name as user_role_name
            FROM users_module.users u
            LEFT JOIN users_module.user_roles ur ON u.id = ur.user_id
            LEFT JOIN users_module.roles r ON r.id = ur.role_id
            WHERE u.id = @id
            FOR UPDATE
            """;

        CommandDefinition command = new(sql, new { id = id.Id }, cancellationToken: ct);
        await using NpgsqlConnection connection = await database.DataSource.OpenConnectionAsync(ct);
        IEnumerable<UserData> rows = await connection.QueryAsync<UserData>(command);
        return !rows.Any() ? null : rows.ToIdentityUser();
    }

    public Task<IdentityUser?> Get(IdentityTokenId id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<IdentityUser>> Get(RoleName role, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
