using System.Data;
using System.Data.Common;
using Dapper;
using Identity.Domain.Contracts;
using Identity.Domain.Permissions;
using Npgsql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Permissions;

public sealed class PermissionsRepository(NpgSqlSession session, IAccountsModuleUnitOfWork unitOfWork) : IPermissionsRepository
{
    private NpgSqlSession Session { get; } = session;
    private IAccountsModuleUnitOfWork UnitOfWork { get; } = unitOfWork;

    public async Task<bool> Exists(PermissionSpecification specification, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(specification);
        string sql = $"""
                           SELECT EXISTS (
                           SELECT 1
                           FROM identity_module.permissions
                           {filterSql}
                           )
                           """;
        
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        return await Session.QuerySingleRow<bool>(command);
    }

    public async Task Add(Permission permission, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO identity_module.permissions
                           (id, name, description)
                           VALUES
                           (@id, @name, @description)
                           """;

        object parameters = new
        {
            id = permission.Id.Value,
            name = permission.Name.Value,
            description = permission.Description.Value
        };

        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        await Session.Execute(command);
    }

    public async Task Add(IEnumerable<Permission> permissions, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO identity_module.permissions
                           (id, name, description)
                           VALUES
                           (@id, @name, @description)
                           """;

        var parameters = permissions.Select(p => new
        {
            id = p.Id.Value,
            name = p.Name.Value,
            description = p.Description.Value
        });

        NpgsqlConnection connection = await Session.GetConnection(ct);
        await connection.ExecuteAsync(sql, parameters, transaction: Session.Transaction);
    }

    public async Task<Result<Permission>> GetSingle(
        PermissionSpecification specification,
        CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(specification);
        string sql = $"""
                      SELECT
                      id as id,
                      name as name,
                      description as description
                      FROM identity_module.permissions
                      WHERE {filterSql}
                      LIMIT 1
                      """;
        
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        Permission? permission = await GetSingle(command, ct);
        
        if (permission is null) 
            return Error.NotFound("Разрешение не найдено.");
        if (specification.LockRequired) 
            await BlockPermission(permission, ct);
        
        UnitOfWork.Track([permission]);
        return Result.Success(permission);
    }

    public async Task<IEnumerable<Permission>> GetMany(
        IEnumerable<PermissionSpecification> specifications,
        CancellationToken ct = default)
    {
        List<string> whereClauses = [];
        DynamicParameters parameters = new();
        int index = 0;
        foreach (PermissionSpecification specification in specifications)
        {
            List<string> innerFilter = [];
            if (specification.Id.HasValue)
            {
                string paramName = $"@permissionId_{index}";
                innerFilter.Add($"id={paramName}");
                parameters.Add(paramName, specification.Id.Value, DbType.Guid);
            }
            
            if (!string.IsNullOrWhiteSpace(specification.Name))
            {
                string paramName = $"@name_{index}";
                innerFilter.Add($"name={paramName}");
                parameters.Add(paramName, specification.Name, DbType.String);
            }
            
            if (innerFilter.Count > 0) whereClauses.Add($"({string.Join(" AND ", innerFilter)})");
            index++;
        }
        
        string filterSql = whereClauses.Count == 0 ? string.Empty : $"WHERE {string.Join(" OR ", whereClauses)}";
        
        string sql = $"""
                      SELECT
                      id as id,
                      name as name,
                      description as description
                      FROM identity_module.permissions
                      {filterSql}
                      """;
        
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        IEnumerable<Permission> permissions = await GetMultiple(command, ct);
        if (!permissions.Any()) return [];
        UnitOfWork.Track(permissions);
        return permissions;
    }

    private async Task BlockPermission(Permission permission, CancellationToken ct = default)
    {
        const string sql = "SELECT id FROM identity_module.permissions WHERE id = @permissionId FOR UPDATE";
        DynamicParameters parameters = new();
        parameters.Add("@permissionId", permission.Id.Value, DbType.Guid);
        CommandDefinition command = new(sql, parameters, transaction: Session.Transaction, cancellationToken: ct);
        await Session.Execute(command);
    }
    
    private async Task<IEnumerable<Permission>> GetMultiple(CommandDefinition command, CancellationToken ct)
    {
        Dictionary<Guid, Permission> mappings = [];
        NpgsqlConnection connection = await Session.GetConnection(ct);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetValue<Guid>("id");
            if (!mappings.TryGetValue(id, out Permission? permission))
            {
                string name = reader.GetValue<string>("name");
                string description = reader.GetValue<string>("description");
                
                permission = new Permission(
                    PermissionId.Create(id), 
                    PermissionName.Create(name), 
                    PermissionDescription.Create(description));
                
                mappings.Add(id, permission);
            }
        }
        
        return mappings.Values;
    }
    
    private async Task<Permission?> GetSingle(CommandDefinition command, CancellationToken ct)
    {
        Dictionary<Guid, Permission> mappings = [];
        NpgsqlConnection connection = await Session.GetConnection(ct);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetValue<Guid>("id");
            if (!mappings.TryGetValue(id, out Permission? permission))
            {
                string name = reader.GetValue<string>("name");
                string description = reader.GetValue<string>("description");
                
                permission = new Permission(
                    PermissionId.Create(id), 
                    PermissionName.Create(name), 
                    PermissionDescription.Create(description));
                
                mappings.Add(id, permission);
            }
        }
        return mappings.Count == 0 ? null : mappings.First().Value;
    }
    
    private (DynamicParameters parameters, string filterSql) WhereClause(PermissionSpecification specification)
    {
        DynamicParameters parameters = new();
        List<string> filterSql = [];

        if (specification.Id.HasValue)
        {
            parameters.Add("@permissionId", specification.Id.Value, DbType.Guid);
            filterSql.Add("id=@permissionId");
        }

        if (!string.IsNullOrWhiteSpace(specification.Name))
        {
            parameters.Add("@name", specification.Name, DbType.String);
            filterSql.Add("name=@name");
        }
        
        return (parameters, filterSql.Count == 0 ? string.Empty : $"WHERE {string.Join(" AND ", filterSql)}");
    }
}