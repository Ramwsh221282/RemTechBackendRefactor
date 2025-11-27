using System.Data;
using Dapper;
using Identity.Application.AccountPermissions;
using Identity.Contracts.AccountPermissions;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Identity.Infrastructure.ACL;

public sealed class QueryAccountAndPermission(NpgSqlSession session)
{
    public async Task<AccountPermission?> Query(
        Guid accountId, 
        Guid permissionId, 
        CancellationToken ct = default)
    {
        const string sql = """
                           SELECT 
                           a.id as account_id,
                           a.name as account_name,
                           a.email as account_email,
                           p.id as permission_id,
                           p.name as permission_name
                           FROM identity_module.accounts a
                           INNER JOIN identity_module.permissions p ON p.id = @permissionId
                           WHERE a.id = @accountId
                           FOR UPDATE
                           """;
        DynamicParameters parameters = new();
        parameters.Add("@accountId", accountId, DbType.Guid);
        parameters.Add("@permissionId", permissionId, DbType.Guid);
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        AccountAndPermissionQueryResult? result = await session.QueryMaybeRow<AccountAndPermissionQueryResult?>(command);
        if (result == null) return null;
        return new AccountPermission(new AccountPermissionData(
            result.AccountId, 
            result.PermissionId, 
            result.AccountEmail, 
            result.AccountName, 
            result.PermissionName
        ));
    }

    private sealed record AccountAndPermissionQueryResult(
        Guid AccountId,
        Guid PermissionId,
        string AccountEmail,
        string PermissionName,
        string AccountName);
}