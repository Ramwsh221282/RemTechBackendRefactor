using System.Data;
using Identity.Adapter.Storage.Storages.Extensions;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Events;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Identity.Adapter.Storage.Storages;

public sealed class UsersStorage(PostgresDatabase database) : IUsersStorage
{
    public async Task<IdentityUser?> Get(UserEmail email, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct: ct);
        return await connection.QueryUser(
            ["u.email = @email"],
            new { email = email.Email },
            ct: ct
        );
    }

    public async Task<IdentityUser?> Get(UserLogin login, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct);
        return await connection.QueryUser(["u.name = @name"], new { name = login.Name }, ct: ct);
    }

    public async Task<IdentityUser?> Get(UserId id, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct);
        return await connection.QueryUser(["u.id = @id"], new { id = id.Id }, ct: ct);
    }

    public async Task<IdentityUser?> Get(IdentityTokenId id, CancellationToken ct = default) =>
        null;

    public async Task<IEnumerable<IdentityUser>> Get(RoleName role, CancellationToken ct = default)
    {
        using IDbConnection connection = await database.ProvideConnection(ct);
        return await connection.QueryUsers(["r.name = @name"], new { name = role.Value }, ct: ct);
    }

    public async Task<Status<IdentityUser>> GetUserWithBlock(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid userId,
        CancellationToken ct
    )
    {
        IdentityUser? user = await connection.QueryUser(
            ["u.id = @userId"],
            new { userId },
            transaction: transaction,
            true,
            ct
        );

        return user == null ? Error.NotFound("Пользователь не найден.") : user;
    }
}
