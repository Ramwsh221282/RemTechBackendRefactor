namespace Identity.Domain.Permissions;

public sealed class Permission(PermissionId id, PermissionName name, PermissionDescription description)
{
    private Permission(Permission permission) : this(permission.Id, permission.Name, permission.Description)
    { }

    public PermissionId Id { get; } = id;
    public PermissionName Name { get; private set; } = name;
    public PermissionDescription Description { get; private set; } = description;
    public void Rename(PermissionName newName) => Name = newName;
    public void ChangeDescription(PermissionDescription newDescription) => Description = newDescription;
    public Permission Clone() => new(this);

    public static Permission CreateNew(PermissionName name, PermissionDescription description)
    {
        PermissionId id = PermissionId.New();
        return new Permission(id, name, description);
    }
}