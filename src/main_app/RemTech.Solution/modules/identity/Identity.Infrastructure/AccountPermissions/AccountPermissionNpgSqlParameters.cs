using System.Data;
using Dapper;
using Identity.Contracts.AccountPermissions.Contracts;

namespace Identity.Infrastructure.AccountPermissions;

public sealed class AccountPermissionNpgSqlParameters
{
    private readonly DynamicParameters _parameters = new();
    private void AddAccountId(Guid id) => _parameters.Add("@account_id", id, DbType.Guid);
    private void AddPermissionId(Guid id) => _parameters.Add("@permission_id", id, DbType.Guid);
    
    public DynamicParameters Read() => _parameters;

    public AccountPermissionNpgSqlParameters(IAccountPermission accountPermission)
    {
        accountPermission.Write(AddAccountId, AddPermissionId);
    }
}