using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Models;

public sealed record IdentitySubjectPermissions
{
    public const int MAX_NAME_LENGTH = 128;
    private readonly IdentitySubjectPermission[] _permissions;

    public Result<IdentitySubjectPermissions> Add(IdentitySubjectPermission permission)
    {
        if (ContainsPermission(permission)) return Conflict("Разрешение уже существует.");
        IdentitySubjectPermission[] @new = [permission, .._permissions];
        return new IdentitySubjectPermissions(@new);
    }

    public Result<IdentitySubjectPermissions> Remove(IdentitySubjectPermission permission)
    {
        if (!ContainsPermission(permission)) return Conflict("Разрешения не существует.");
        IdentitySubjectPermission[] @new = 
        [
            .._permissions.Where(p => p.Id != permission.Id || p.Name != permission.Name)
        ];
        return new IdentitySubjectPermissions(@new);
    }

    public void ForEach(Action<IdentitySubjectPermission> action)
    {
        foreach (IdentitySubjectPermission role in _permissions)
            action(role);
    }
    
    private bool ContainsPermission(IdentitySubjectPermission permission)
    {
        return _permissions.Any(p => p.Id == permission.Id || p.Name == permission.Name);
    }
        
    
    internal IdentitySubjectPermissions()
    {
        _permissions = [];
    }
    
    internal IdentitySubjectPermissions(IEnumerable<IdentitySubjectPermission> permissions)
    {
        _permissions = [..permissions];
    }
}