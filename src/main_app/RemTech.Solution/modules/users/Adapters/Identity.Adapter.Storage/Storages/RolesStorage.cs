using System.Data;
using Dapper;
using Identity.Adapter.Storage.DataModels;
using Identity.Domain.Roles;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using Shared.Infrastructure.Module.Postgres;

namespace Identity.Adapter.Storage.Storages;

internal static class IdentityRoleDataModelExtensions
{
    public static IdentityRole? TryMapSingle(this IEnumerable<IdentityRoleDataModel> rows) =>
        !rows.Any() ? null : rows.First().Map();

    public static IdentityRole Map(this IdentityRoleDataModel dm) =>
        IdentityRole.Create(RoleName.Create(dm.Name), RoleId.Create(dm.Id));
}

public sealed class RolesStorage(PostgresDatabase database) : IRolesStorage
{
    public async Task<IdentityRole?> Get(RoleName name, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
            id,
            name
            FROM users_module.roles
            WHERE name = @name
            """;
        CommandDefinition command = new(sql, new { name = name.Value }, cancellationToken: ct);
        using IDbConnection connection = await database.ProvideConnection(ct: ct);
        return (await connection.QueryAsync<IdentityRoleDataModel>(command)).TryMapSingle();
    }

    public async Task<IdentityRole?> Get(RoleId id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
            id,
            name
            FROM users_module.roles
            WHERE id = @id
            """;
        CommandDefinition command = new(sql, new { id = id.Value }, cancellationToken: ct);
        using IDbConnection connection = await database.ProvideConnection(ct: ct);
        return (await connection.QueryAsync<IdentityRoleDataModel>(command)).TryMapSingle();
    }
}
