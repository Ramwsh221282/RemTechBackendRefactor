using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions;

namespace Identity.Core.SubjectsModule.Models;

public sealed record SubjectPermissions
{
    public const int MAX_NAME_LENGTH = 128;
    private readonly IdentitySubjectPermission[] _permissions;

    public Result<SubjectPermissions> Add(IdentitySubjectPermission permission)
    {
        if (ContainsPermission(permission)) return Conflict("Разрешение уже существует.");
        IdentitySubjectPermission[] @new = [permission, .._permissions];
        return new SubjectPermissions(@new);
    }

    public Result<SubjectPermissions> Remove(IdentitySubjectPermission permission)
    {
        if (!ContainsPermission(permission)) return Conflict("Разрешения не существует.");
        IdentitySubjectPermission[] @new = 
        [
            .._permissions.Where(p => p.Id != permission.Id || p.Name != permission.Name)
        ];
        return new SubjectPermissions(@new);
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

    public IdentitySubjectPermissionSnapshot[] Snapshotted()
    {
        return _permissions.Map(p => new IdentitySubjectPermissionSnapshot(p.Id, p.Name));
    }
    
    internal SubjectPermissions()
    {
        _permissions = [];
    }
    
    internal SubjectPermissions(IEnumerable<IdentitySubjectPermission> permissions)
    {
        _permissions = [..permissions];
    }
}