using Identity.Contracts.Permissions;
using Identity.Contracts.Permissions.Contracts;
using Identity.Contracts.Permissions.Defaults;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.Permissions;

public sealed class Permission(PermissionData data)
{
    private readonly PermissionData _data = data;
    private readonly IOnPermissionCreatedEventListener _onCreated = new NoneOnPermissionCreatedEventListener();
    private readonly IOnPermissionRenamedEventListener _onRenamed = new NoneOnPermissionRenamedEventListener();
    
    public async Task<Result<Unit>> Register(CancellationToken ct = default)
    {
        Result<Unit> validation =  ValidateState();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> eventHandling = await _onCreated.React(_data, ct);
        if (eventHandling.IsFailure) return eventHandling.Error;
        return Unit.Value;
    }
    
    public async Task<Result<Permission>> Rename(string newName, CancellationToken ct = default)
    {
        Permission renamed = new Permission(_data with { Name = newName })
            .AddListener(_onCreated)
            .AddListener(_onRenamed);
        Result<Unit> validation = renamed.ValidateState();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> eventHandling = await _onRenamed.React(renamed._data, ct);
        if (eventHandling.IsFailure) return eventHandling.Error;
        return renamed;
    }

    public void Write(Action<Guid> writeId, Action<string> writeName)
    {
        writeId(_data.Id);
        writeName(_data.Name);
    }

    public Permission AddListener(IOnPermissionCreatedEventListener listener) => new(this, listener);

    public Permission AddListener(IOnPermissionRenamedEventListener listener) => new(this, listener);

    private Result<Unit> ValidateState()
    {
        const string valueName = "Название разрешения";
        const int maxLength = 128;
        List<string> errors = [];
        if (string.IsNullOrWhiteSpace(_data.Name)) errors.Add(Error.NotSet(valueName));
        if (_data.Name.Length > maxLength) errors.Add(Error.GreaterThan(valueName, maxLength));
        if (errors.Count > 0) return Error.Validation(errors);
        return Unit.Value;
    }

    public bool HasName(string name)
    {
        return _data.Name == name;
    }

    public bool HasId(Guid id)
    {
        return _data.Id == id;
    }
    
    private Permission(Permission permission, IOnPermissionCreatedEventListener listener) 
        : this(permission._data) => _onCreated = listener;
    
    private Permission(Permission permission, IOnPermissionRenamedEventListener listener) 
        : this(permission._data) => _onRenamed = listener;
}