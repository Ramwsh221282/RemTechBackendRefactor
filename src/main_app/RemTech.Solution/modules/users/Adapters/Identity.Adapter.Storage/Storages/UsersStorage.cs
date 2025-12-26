using System.Data;
using Dapper;
using Identity.Adapter.Storage.Storages.Extensions;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Entities.Tickets.ValueObjects;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Identity.Adapter.Storage.Storages;

public sealed class UsersStorage(PostgresDatabase database) : IUsersStorage
{
    public async Task<User?> Get(UserEmail email, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct: ct);
        return await connection.QueryUser(
            ["u.email = @email"],
            parameters: new { email = email.Email },
            ct: ct
        );
    }

    public async Task<User?> Get(UserLogin login, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct);
        return await connection.QueryUser(
            ["u.name = @name"],
            parameters: new { name = login.Name },
            ct: ct
        );
    }

    public async Task<User?> Get(UserId id, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct);
        return await connection.QueryUser(["u.id = @id"], parameters: new { id = id.Id }, ct: ct);
    }

    public async Task<User?> Get(UserTicketId id, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct);
        return await connection.QueryUser(["t.id = @id"], parameters: new { id = id.Id }, ct: ct);
    }

    public async Task<IEnumerable<User>> Get(RoleName role, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct);
        return await connection.QueryUsers(
            ["r.name = @name"],
            parameters: new { name = role.Value },
            ct: ct
        );
    }

    public async Task<IEnumerable<User>> Get(
        UsersSpecification specification,
        CancellationToken ct = default
    )
    {
        List<string> whereClauses = [];
        List<string> orderByClauses = [];
        List<string> paginationClauses = [];
        var parameters = new DynamicParameters();
        if (!string.IsNullOrWhiteSpace(specification.Email))
        {
            whereClauses.Add("u.email ILIKE @email");
            parameters.Add("@email", $"%{specification.Email}%", DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(specification.Login))
        {
            whereClauses.Add("u.name ILIKE @login");
            parameters.Add("@login", $"%{specification.Login}%", DbType.String);
        }

        if (specification.VerifiedOnly != null)
        {
            whereClauses.Add("u.email_confirmed = @verified");
            parameters.Add("@verified", specification.VerifiedOnly.Value, DbType.Boolean);
        }

        if (specification.Roles != null)
        {
            int roleIdx = 0;
            foreach (string role in specification.Roles)
            {
                whereClauses.Add($"r.name = @role{roleIdx}");
                parameters.Add($"@role{roleIdx}", role, DbType.String);
            }
        }

        if (specification.OrderClauses != null)
        {
            string mode = specification.OrderMode;
            foreach (string orderClause in specification.OrderClauses)
            {
                string clause = orderClause switch
                {
                    "login" => $"u.name {mode}",
                    "email" => $"u.email {mode}",
                    "role" => $"r.name {mode}",
                    _ => string.Empty,
                };

                orderByClauses.Add(clause);
            }
        }

        int limit = specification.PageSize;
        int offset = (specification.Page - 1) * limit;
        paginationClauses.Add("LIMIT @limit");
        paginationClauses.Add("OFFSET @offset");
        parameters.Add("@limit", limit, DbType.Int32);
        parameters.Add("@offset", offset, DbType.Int32);

        using var connection = await database.ProvideConnection(ct);
        return await connection.QueryUsers(
            whereClauses,
            orderByClauses,
            paginationClauses,
            parameters,
            ct: ct
        );
    }

    public async Task<Status<User>> GetUserWithBlock(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid userId,
        CancellationToken ct
    )
    {
        User? user = await connection.QueryUser(
            ["u.id = @userId"],
            parameters: new { userId },
            transaction: transaction,
            true,
            ct
        );

        return user == null ? Error.NotFound("Пользователь не найден.") : user;
    }
}
