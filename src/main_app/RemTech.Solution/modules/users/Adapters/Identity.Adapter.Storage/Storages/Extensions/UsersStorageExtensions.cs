using System.Data;
using Dapper;
using Identity.Adapter.Storage.Storages.Models;
using Identity.Domain.Users.Aggregate;

namespace Identity.Adapter.Storage.Storages.Extensions;

public static class UsersStorageExtensions
{
    public static async Task<IEnumerable<User>> QueryUsers(
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
        var data = await connection.QueryAsync<UserData, RoleData, UserTicketData?, UserData>(
            command,
            (user, role, ticket) => MapUser(users, user, role, ticket),
            splitOn: "role_id,ticket_id"
        );
        return data.Select(d => d.ToIdentityUser());
    }

    public static async Task<User?> QueryUser(
        this IDbConnection connection,
        List<string> whereClauses,
        object? parameters = null,
        IDbTransaction? transaction = null,
        bool withLock = false,
        CancellationToken ct = default
    )
    {
        IEnumerable<User> users = await connection.QueryUsers(
            whereClauses,
            parameters,
            transaction,
            withLock,
            ct: ct
        );

        return !users.Any() ? null : users.First();
    }

    private static UserData MapUser(
        Dictionary<Guid, UserData> users,
        UserData user,
        RoleData role,
        UserTicketData? ticket
    )
    {
        if (!users.TryGetValue(user.Id, out UserData? userData))
        {
            userData = user;
            users.Add(user.Id, userData);
        }

        if (ticket != null && ticket.UserId == userData.Id)
            userData.Tickets.Add(ticket);

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
            r.name as role_name,
            t.id as ticket_id,
            t.user_id as ticket_user_id,
            t.type as ticket_type,
            t.created as ticket_created,
            t.expired as ticket_expired,
            t.deleted as ticket_deleted
            FROM users_module.users u
            INNER JOIN users_module.user_roles ur ON u.id = ur.user_id
            INNER JOIN users_module.roles r ON r.id = ur.role_id
            LEFT JOIN users_module.tickets t ON u.id = t.user_id
            {whereClause}
            {transactionClause}
            """;
    }
}
