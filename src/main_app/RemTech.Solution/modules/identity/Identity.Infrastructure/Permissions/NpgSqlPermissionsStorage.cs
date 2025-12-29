using System.Data;
using Identity.Application.Permissions;
using Identity.Application.Permissions.Contracts;
using Identity.Contracts.Permissions;

namespace Identity.Infrastructure.Permissions;

public sealed class NpgSqlPermissionsStorage(NpgSqlSession session) : IPermissionsStorage
{
    public async Task Persist(Permission instance, CancellationToken ct = default)
    {
        const string sql = "INSERT INTO identity_module.permissions(id, name) VALUES(@id, @name)";
        PermissionDataNpgSqlParameters parameters = PermissionDataNpgSqlParameters.Create(instance);
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }

    public async Task<Permission?> Fetch(PermissionQueryArgs args, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(args);
        string lockClause =  LockClause(args);
        string sql =
            $"""
            SELECT * FROM identity_module.permissions
            {filterSql}
            {lockClause}
            """;
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlPermissionRow? row = await session.QueryMaybeRow<NpgSqlPermissionRow?>(command);
        return row?.ToPermission();
    }

    public async Task Update(Permission instance, CancellationToken ct = default)
    {
        const string sql = "UPDATE identity_module.permissions SET name=@name WHERE id=@id";
        PermissionDataNpgSqlParameters parameters = PermissionDataNpgSqlParameters.Create(instance);
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }

    private static string LockClause(PermissionQueryArgs args)
    {
        return args.WithLock ? "FOR UPDATE" : string.Empty;
    }
    
    private static (DynamicParameters parameters, string filterSql) WhereClause(PermissionQueryArgs args)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();
        if (args.Id.HasValue)
        {
            filters.Add("id=@id");
            parameters.Add("@id", args.Id.Value, DbType.Guid);
        }

        if (!string.IsNullOrEmpty(args.Name))
        {
            filters.Add("name=@name");
            parameters.Add("@name", args.Name, DbType.String);
        }
        
        string sql = filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);
        return (parameters, sql);
    }

    private sealed record NpgSqlPermissionRow(Guid Id, string Name)
    {
        public Permission ToPermission()
        {
            return new Permission(new PermissionData(Id, Name));
        }
    }
}