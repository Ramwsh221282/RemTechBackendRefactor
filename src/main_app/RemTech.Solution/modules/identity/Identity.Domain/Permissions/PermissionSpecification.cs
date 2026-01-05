namespace Identity.Domain.Permissions;

public sealed class PermissionSpecification
{
    public Guid? Id { get; private set; }
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public bool LockRequired { get; private set; } = false;

    public PermissionSpecification WithId(Guid id)
    {
        if (Id.HasValue) return this;
        Id = id;
        return this;
    }

    public PermissionSpecification WithName(string name)
    {
        if (!string.IsNullOrWhiteSpace(Name)) return this;
        Name = name;
        return this;
    }

    public PermissionSpecification WithDescription(string description)
    {
        if (!string.IsNullOrWhiteSpace(Description)) return this;
        Description = description;
        return this;
    }

    public PermissionSpecification WithLock()
    {
        LockRequired = true;
        return this;
    }
}