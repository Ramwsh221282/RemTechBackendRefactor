using System.Data;
using Identity.Application.Permissions;

namespace Identity.Infrastructure.Permissions;

public sealed class PermissionDataNpgSqlParameters
{
    private readonly DynamicParameters _parameters;

    private void AddId(Guid id) =>
        _parameters.Add("@id", id, DbType.Guid);

    private void AddName(string name) =>
        _parameters.Add("@name", name, DbType.String);
    
    public DynamicParameters Read()
    {
        return !_parameters.ParameterNames.Any()
            ? throw new InvalidOperationException(
                $"{nameof(PermissionDataNpgSqlParameters)} has no parameters injected.")
            : _parameters;
    }

    private PermissionDataNpgSqlParameters() =>
        _parameters = new DynamicParameters();

    public static PermissionDataNpgSqlParameters Create(Permission permission)
    {
        PermissionDataNpgSqlParameters parameters = new();
        permission.Write(parameters.AddId, parameters.AddName);
        return parameters;
    }
}