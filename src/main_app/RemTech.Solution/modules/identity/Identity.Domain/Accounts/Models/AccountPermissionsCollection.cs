using Identity.Domain.Permissions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

public sealed class AccountPermissionsCollection(AccountId id, IEnumerable<Permission> permissions)
{
    private AccountPermissionsCollection(AccountPermissionsCollection collection)
        : this(collection.AccountId, [.. collection._permissions]) { }

    private readonly List<Permission> _permissions = [.. permissions];
    public AccountId AccountId { get; } = id;
    public IReadOnlyList<Permission> Permissions => _permissions;
    public int Count => _permissions.Count;

    public Result<Unit> Add(Permission permission)
    {
        if (HasPermission(permission))
            return Error.Conflict("Разрешение уже присвоено учетной записи.");
        _permissions.Add(permission);
        return Result.Success(Unit.Value);
    }

    public Result<Unit> Remove(Permission permission)
    {
        if (!HasPermission(permission))
            return Error.NotFound("Разрешение не найдено.");
        _permissions.Remove(permission);
        return Result.Success(Unit.Value);
    }

    public Result<Permission> Find(Guid permissionId)
    {
        Permission? permission = _permissions.FirstOrDefault(p => p.Id.Value == permissionId);
        return permission is null ? Error.NotFound("Разрешение не найдено.") : Result.Success(permission);
    }

    public Result<Permission> Find(string permissionName)
    {
        Permission? permission = _permissions.FirstOrDefault(p => p.Name.Value == permissionName);
        return permission is null ? Error.NotFound("Разрешение не найдено.") : Result.Success(permission);
    }

    private bool HasPermission(Permission permission) => _permissions.Any(p => p.Id == permission.Id);

    public AccountPermissionsCollection Clone() => new(this);

    public static AccountPermissionsCollection Empty(AccountId id) => new(id, []);
}
