using Identity.Application.AccountPermissions.Defaults;
using Identity.Contracts.AccountPermissions;
using Identity.Contracts.AccountPermissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.AccountPermissions;

public sealed class AccountPermission(AccountPermissionData data) : IAccountPermission
{
    private readonly IOnAccountPermissionCreatedEventListener _onCreated =
        new NoneOnAccountPermissionRegisteredEventListener();
    
    private readonly IOnAccountPermissionRemovedEventListener _onRemoved =
        new NoneOnAccountPermissionRemovedEventListener();
    
    public async Task<Result<Unit>> Save(CancellationToken ct = default)
    {
        Result<Unit> validation = ValidateState();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> handling = await _onCreated.React(data, ct);
        return handling;
    }
    
    public AccountPermission AddListener(IOnAccountPermissionCreatedEventListener onCreated) => new(data, onCreated);
    public AccountPermission AddListener(IOnAccountPermissionRemovedEventListener onRemoved) => new(data, onRemoved);
    
    public async Task<Result<Unit>> Delete(CancellationToken ct = default)
    {
        Result<Unit> validation = ValidateState();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> handling = await _onRemoved.React(data, ct);
        return handling;
    }

    private Result<Unit> ValidateState()
    {
        const string permIdName = "Идентификатор разрешения";
        const string accIdName = "Идентификатор учетной записи";
        List<string> errors = [];
        if (data.PermissionId == Guid.Empty) errors.Add(Error.NotSet(permIdName));
        if (data.AccountId == Guid.Empty) errors.Add(Error.NotSet(accIdName));
        return errors.Count == 0 ? Unit.Value : Error.Validation(errors);
    }
    
    public Result<Unit> Write(
        Action<Guid>? writePermissionId = null,
        Action<Guid>? writeAccountId = null,
        Action<string>? writeAccountName = null,
        Action<string>? writeAccountEmail = null,
        Action<string>? writePermissionName = null)
    {
        Result<Unit> validation = ValidateState();
        if (validation.IsFailure) return validation.Error;
        writePermissionId?.Invoke(data.PermissionId);
        writeAccountId?.Invoke(data.AccountId);
        writeAccountName?.Invoke(data.AccountName);
        writeAccountEmail?.Invoke(data.Email);
        writePermissionName?.Invoke(data.PermissionName);
        return validation;
    }

    private AccountPermission(AccountPermissionData data, IOnAccountPermissionCreatedEventListener onCreated)
    : this(data) => _onCreated = onCreated;
    
    private AccountPermission(AccountPermissionData data, IOnAccountPermissionRemovedEventListener onRemoved)
        : this(data) => _onRemoved = onRemoved;
}