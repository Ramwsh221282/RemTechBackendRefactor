using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions;

namespace Identity.Core.SubjectsModule.Domain.Permissions;

public sealed record SubjectPermissions
{
    private readonly SubjectPermission[] _permissions;

    public Result<SubjectPermissions> Add(SubjectPermission permission)
    {
        if (ContainsPermission(permission)) return Conflict("Разрешение уже существует.");
        SubjectPermission[] @new = [permission, .._permissions];
        return new SubjectPermissions(@new);
    }

    public Result<SubjectPermissions> Remove(SubjectPermission permission)
    {
        if (!ContainsPermission(permission)) return Conflict("Разрешения не существует.");
        SubjectPermission[] @new = 
        [
            .._permissions.Where(p => p.Id != permission.Id || p.Name != permission.Name)
        ];
        return new SubjectPermissions(@new);
    }

    public void ForEach(Action<SubjectPermission> action)
    {
        foreach (SubjectPermission role in _permissions)
            action(role);
    }
    
    private bool ContainsPermission(SubjectPermission permission)
    {
        return _permissions.Any(p => p.Id == permission.Id || p.Name == permission.Name);
    }

    public SubjectPermissionSnapshot[] Snapshotted()
    {
        return _permissions.Map(p => new SubjectPermissionSnapshot(p.Id, p.Name));
    }
    
    internal SubjectPermissions()
    {
        _permissions = [];
    }
    
    internal SubjectPermissions(IEnumerable<SubjectPermission> permissions)
    {
        _permissions = [..permissions];
    }
}