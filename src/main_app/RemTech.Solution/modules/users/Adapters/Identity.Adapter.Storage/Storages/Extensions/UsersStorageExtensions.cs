using System.Data;
using Dapper;
using Identity.Adapter.Storage.Storages.Models;
using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

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

    public static DynamicParameters TicketInsertParams(
        this DynamicParameters parameters,
        Guid id,
        Guid userId,
        string type,
        DateTime created,
        DateTime expired
    )
    {
        parameters.Add("@id", id, DbType.Guid);
        parameters.Add("@user_id", userId, DbType.Guid);
        parameters.Add("@type", type, DbType.String);
        parameters.Add("@created", created, DbType.DateTime);
        parameters.Add("@expired", expired, DbType.DateTime);
        return parameters;
    }

    public static async Task InsertTicket(
        this IDbConnection connection,
        DynamicParameters parameters,
        IDbTransaction transaction,
        CancellationToken ct
    )
    {
        const string sql = """
            INSERT INTO users_module.tickets
            (id, user_id, type, created, expired)
            VALUES
            (@id, @user_id, @type, @created, @expired)
            """;

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: ct,
            transaction: transaction
        );

        await connection.ExecuteAsync(command);
    }

    public static async Task<Status> UpdateUserRow(
        this IDbConnection connection,
        IDbTransaction transaction,
        IEnumerable<string> setClauses,
        IEnumerable<string> whereClauses,
        DynamicParameters parameters,
        CancellationToken ct = default
    )
    {
        if (!setClauses.Any())
            return Status.Internal("No set clauses provided for user row update.");
        if (!whereClauses.Any())
            return Status.Internal("No where clauses provided for user row update.");

        string setClause = setClauses.Any() ? "SET " + string.Join(", ", setClauses) : string.Empty;
        string whereClause = whereClauses.Any()
            ? "WHERE " + string.Join(" AND ", whereClauses)
            : string.Empty;
        string sql = $"UPDATE users {setClause} {whereClause}";

        CommandDefinition command = new(
            sql,
            parameters,
            cancellationToken: ct,
            transaction: transaction
        );

        int affected = await connection.ExecuteAsync(command);
        return affected > 0 ? Status.Success() : Status.Internal("No rows affected.");
    }

    public static async Task<Status> BlockTicketedUserRow(
        this IDbConnection connection,
        Guid userId,
        Guid ticketId,
        IDbTransaction transaction,
        CancellationToken ct = default
    )
    {
        const string sql = """
            SELECT 1
            FROM users_module.users u
            INNER JOIN users_module.tickets t ON u.id = t.user_id
            WHERE u.id = @user_id AND t.id = @ticket_id
            FOR UPDATE OF u, t                                      
            """;

        var command = new CommandDefinition(
            sql,
            new { user_id = userId, ticket_id = ticketId },
            cancellationToken: ct,
            transaction: transaction
        );

        int? exists = await connection.QueryFirstOrDefaultAsync<int?>(command);
        return exists == null
            ? Status.NotFound("Пользователь по заявке не найден.")
            : Status.Success();
    }

    public static async Task<Status> BlockUserRow(
        this IDbConnection connection,
        Guid userId,
        IDbTransaction transaction,
        CancellationToken ct = default
    )
    {
        const string sql = """
            SELECT 1
            FROM users_module.users u
            WHERE u.id = @user_id
            FOR UPDATE OF u                                      
            """;

        var command = new CommandDefinition(
            sql,
            new { user_id = userId },
            cancellationToken: ct,
            transaction: transaction
        );

        int? exists = await connection.QueryFirstOrDefaultAsync<int?>(command);
        return exists == null
            ? Status.NotFound("Пользователь по заявке не найден.")
            : Status.Success();
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
