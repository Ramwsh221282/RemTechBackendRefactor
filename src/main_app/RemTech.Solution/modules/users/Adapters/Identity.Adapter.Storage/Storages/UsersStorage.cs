using System.Data;
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
            new { email = email.Email },
            ct: ct
        );
    }

    public async Task<User?> Get(UserLogin login, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct);
        return await connection.QueryUser(["u.name = @name"], new { name = login.Name }, ct: ct);
    }

    public async Task<User?> Get(UserId id, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct);
        return await connection.QueryUser(["u.id = @id"], new { id = id.Id }, ct: ct);
    }

    public async Task<User?> Get(UserTicketId id, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct);
        return await connection.QueryUser(["t.id = @id"], new { id = id.Id }, ct: ct);
    }

    public async Task<IEnumerable<User>> Get(RoleName role, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct);
        return await connection.QueryUsers(["r.name = @name"], new { name = role.Value }, ct: ct);
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
            new { userId },
            transaction: transaction,
            true,
            ct
        );

        return user == null ? Error.NotFound("Пользователь не найден.") : user;
    }
}
