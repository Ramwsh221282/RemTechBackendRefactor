using System.Data;
using Dapper;
using Identity.Adapter.Storage.Storages.Models;
using Identity.Domain.Users.Aggregate;

namespace Identity.Adapter.Storage.Storages.Extensions;

public static class UsersStorageExtensions
{
    public static async Task<IEnumerable<IdentityUser>> QueryUsers(
        this IDbConnection connection,
        List<string> whereClauses,
        object? parameters = null,
        IDbTransaction? transaction = null,
        bool withLock = false,
        CancellationToken ct = default
    )
    {
        string sql = CreateSql(whereClauses, withLock);
        CommandDefinition command = new(
            sql,
            parameters,
            cancellationToken: ct,
            transaction: transaction
        );

        Dictionary<Guid, UserData> users = [];
        return (
            await connection.QueryAsync<UserData, RoleData, UserData>(
                command,
                (user, role) => MapUser(users, user, role),
                splitOn: "role_id"
            )
        ).Select(d => d.ToIdentityUser());
    }

    public static async Task<IdentityUser?> QueryUser(
        this IDbConnection connection,
        List<string> whereClauses,
        object? parameters = null,
        IDbTransaction? transaction = null,
        bool withLock = false,
        CancellationToken ct = default
    )
    {
        IEnumerable<IdentityUser> users = await connection.QueryUsers(
            whereClauses,
            parameters,
            transaction,
            withLock,
            ct: ct
        );

        return !users.Any() ? null : users.First();
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

    private static string CreateSql(List<string> whereClauses, bool transactional = false)
    {
        string whereClause =
            whereClauses.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", whereClauses);

        string transactionClause = transactional ? "FOR UPDATE OF u" : string.Empty;

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
            INNER JOIN users_module.user_roles ur ON u.id = ur.user_id
            INNER JOIN users_module.roles r ON r.id = ur.role_id
            {whereClause}
            {transactionClause}
            """;
    }
}
