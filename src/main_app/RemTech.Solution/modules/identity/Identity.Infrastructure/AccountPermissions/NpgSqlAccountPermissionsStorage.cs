using System.Data;
using Dapper;
using Identity.Application.AccountPermissions;
using Identity.Contracts.AccountPermissions;
using Identity.Contracts.AccountPermissions.Contracts;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Identity.Infrastructure.AccountPermissions;

public sealed class NpgSqlAccountPermissionsStorage(NpgSqlSession session) : IAccountPermissionsStorage
{
    public async Task Persist(IAccountPermission instance, CancellationToken ct = default)
    {
        AccountPermissionNpgSqlParameters parameters = new(instance);
        const string sql = """
                           INSERT INTO identity_module.account_permissions (account_id, permission_id)
                           VALUES (@account_id, @permission_id);
                           """;
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }

    public async Task<IAccountPermission?> Fetch(AccountPermissionQueryArgs args, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filters) = FillParameters(args);
        string lockClause = LockClause(args);
        string sql = $"""
                      SELECT
                      ap.account_id as account_id,
                      ap.permission_id as permission_id,
                      a.name as account_name,
                      a.email as account_email,
                      p.name as permission_name
                      FROM identity_module.account_permissions ap
                      INNER JOIN identity_module.permissions p ON ap.permission_id = p.id
                      INNER JOIN identity_module.accounts a ON ap.account_id = a.id
                      {filters}
                      {lockClause}
                      LIMIT 1
                      """;
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlAccountPermissionRow? row = await session.QueryMaybeRow<NpgSqlAccountPermissionRow?>(command);
        return row?.ToAccountPermission();
    }

    public async Task Remove(IAccountPermission instance, CancellationToken ct = default)
    {
        AccountPermissionNpgSqlParameters parameters = new(instance);
        const string sql = """
                           DELETE FROM identity_module.account_permissions
                           WHERE account_id = @account_id AND permission_id = @permission_id;
                           """;
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }

    private static string LockClause(AccountPermissionQueryArgs args)
    {
        return args.WithLock ? "FOR UPDATE" : string.Empty;
    }
    
    private static (DynamicParameters parameters, string filterSql) FillParameters(AccountPermissionQueryArgs args)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();

        if (args.AccountId.HasValue)
        {
            filters.Add("ap.account_id = @account_id");
            parameters.Add("@account_id", args.AccountId.Value, DbType.Guid);
        }

        if (args.PermissionId.HasValue)
        {
            filters.Add("ap.permission_id = @permission_id");
            parameters.Add("@permission_id", args.PermissionId.Value, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(args.AccountEmail))
        {
            filters.Add("a.email = @account_email");
            parameters.Add("@account_email", args.AccountEmail, DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(args.AccountName))
        {
            filters.Add("a.name = @account_name");
            parameters.Add("@account_name", args.AccountName, DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(args.PermissionName))
        {
            filters.Add("p.name = @permission_name");
            parameters.Add("@permission_name", args.PermissionName, DbType.String);
        }
        
        string sql = filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);
        return (parameters, sql);
    }

    private sealed record NpgSqlAccountPermissionRow(
        Guid AccountId,
        Guid PermissionId,
        string AccountEmail,
        string AccountName,
        string PermissionName)
    {
        public IAccountPermission ToAccountPermission()
        {
            AccountPermissionData data = new(AccountId, PermissionId, AccountEmail, AccountName, PermissionName);
            return new AccountPermission(data);
        }
    }
}