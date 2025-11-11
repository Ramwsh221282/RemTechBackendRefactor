using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule;

public sealed record IdentitySubject(
    IdentitySubjectMetadata Metadata, 
    IdentitySubjectCredentials Credentials, 
    IdentitySubjectActivationStatus Activation)
{
    public void Register()
    {
        throw new NotImplementedException();
    }
}

public sealed record IdentitySubjectMetadata(Guid Id, string Login);

public sealed record IdentitySubjectCredentials(string Email, string Password);

public sealed record IdentitySubjectActivationStatus
{
    private readonly Optional<DateTime> _activationDate;

    public Result<IdentitySubjectActivationStatus> Activate()
    {
        if (_activationDate.HasValue) return Conflict("Учетная запись уже активирована.");
        IdentitySubjectActivationStatus @new = new(DateTime.UtcNow);
        return Success(@new);
    }
    
    internal IdentitySubjectActivationStatus(DateTime activationDate)
    {
        _activationDate = Some(activationDate);
    }
    
    internal IdentitySubjectActivationStatus()
    {
        _activationDate = None<DateTime>();
    }
}

public static class IdentitySubjectsModule
{
    
}

public static class IdentitySubjectRolesModule
{
    extension(IdentitySubjectRolesCollection)
    {
        public static IdentitySubjectRolesCollection Empty()
        {
            return new IdentitySubjectRolesCollection();
        }

        public static Result<IdentitySubjectRolesCollection> Create(IEnumerable<IdentitySubjectRole> roles)
        {
            Result<UniqueSequence<IdentitySubjectRole>> unique = roles.TryBecomeUnique("Список ролей пользователя не уникален.");
            return unique.IsFailure 
                ? unique.Error 
                : new IdentitySubjectRolesCollection(unique.Value.Map(i => i));
        }
    }
}

public static class IdentitySubjectActivationStatusModule
{
    extension(IdentitySubjectActivationStatus)
    {
        public static IdentitySubjectActivationStatus Inactive()
        {
            return new IdentitySubjectActivationStatus();
        }

        public static Result<IdentitySubjectActivationStatus> Create(DateTime activationDate)
        {
            DateTime now = DateTime.UtcNow.AddMinutes(1);
            if (activationDate == DateTime.MinValue || activationDate == DateTime.MaxValue || activationDate > now)
                return Validation("Дата активации учетной записи некорректна.");
            IdentitySubjectActivationStatus @new = new(activationDate);
            return Success(@new);
        }
    }
}

public static class IdentitySubjectRolePermissionsModule
{
    extension(IdentitySubjectRolePermissionsCollection)
    {
        public static IdentitySubjectRolePermissionsCollection Empty()
        {
            return new IdentitySubjectRolePermissionsCollection();
        }

        public static Result<IdentitySubjectRolesCollection> Create(IEnumerable<IdentitySubjectRole> roles)
        {
            Result<UniqueSequence<IdentitySubjectRole>> unique = roles.TryBecomeUnique("Список ролей пользователя не уникален.");
            return unique.IsFailure 
                ? unique.Error 
                : new IdentitySubjectRolesCollection(unique.Value.Map(i => i));
        }
    }
}

public sealed record IdentitySubjectRolesCollection
{
    private readonly IdentitySubjectRole[] _roles;

    internal IdentitySubjectRolesCollection()
    {
        _roles = [];
    }
    
    internal IdentitySubjectRolesCollection(IEnumerable<IdentitySubjectRole> roles)
    {
        _roles = [..roles];
    }

    public int Amount => _roles.Length;
    
    public void ForRole(Action<IdentitySubjectRole> action)
    {
        foreach (var role in _roles)
            action(role);
    }
    
    public Result<IdentitySubjectRolesCollection> Add(IdentitySubjectRole role)
    {
        if (ContainsRole(role)) return Conflict($"Роль {role.Name} уже существует.");
        IReadOnlyList<IdentitySubjectRole> @new = [role, .._roles];
        return Success(new IdentitySubjectRolesCollection(@new));
    }

    public Result<IdentitySubjectRolesCollection> Remove(IdentitySubjectRole role)
    {
        if (!ContainsRole(role)) return Conflict($"Роль {role.Name} не существует.");
        IReadOnlyList<IdentitySubjectRole> @new = [.._roles.Where(r => r.Id != role.Id || r.Name != role.Name)];
        return Success(new IdentitySubjectRolesCollection(@new));
    }

    private bool ContainsRole(IdentitySubjectRole role)
    {
        return _roles.Any(r => r.Id == role.Id || r.Name == role.Name);
    }
}

public sealed record IdentitySubjectRole
{
    internal Guid Id { get; init; }
    internal string Name { get; init; }

    internal IdentitySubjectRole(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}

public sealed record IdentitySubjectRolePermissionsCollection
{
    private readonly IdentitySubjectRolePermission[] _permissions;

    public Result<IdentitySubjectRolePermissionsCollection> Add(IdentitySubjectRolePermission permission)
    {
        if (ContainsPermission(permission)) return Conflict("Разрешение уже существует.");
        IdentitySubjectRolePermission[] @new = [permission, .._permissions];
        return new IdentitySubjectRolePermissionsCollection(@new);
    }

    public Result<IdentitySubjectRolePermissionsCollection> Remove(IdentitySubjectRolePermission permission)
    {
        if (!ContainsPermission(permission)) return Conflict("Разрешения не существует.");
        IdentitySubjectRolePermission[] @new = 
        [
            .._permissions.Where(p => p.Id != permission.Id || p.Name != permission.Name)
        ];
        return new IdentitySubjectRolePermissionsCollection(@new);
    }

    public void ForEach(Action<IdentitySubjectRolePermission> action)
    {
        foreach (IdentitySubjectRolePermission role in _permissions)
            action(role);
    }
    
    private bool ContainsPermission(IdentitySubjectRolePermission permission)
    {
        return _permissions.Any(p => p.Id == permission.Id || p.Name == permission.Name);
    }
        
    
    internal IdentitySubjectRolePermissionsCollection()
    {
        _permissions = [];
    }
    
    internal IdentitySubjectRolePermissionsCollection(IEnumerable<IdentitySubjectRolePermission> permissions)
    {
        _permissions = [..permissions];
    }
}

public sealed record IdentitySubjectRolePermission(Guid Id, string Name);