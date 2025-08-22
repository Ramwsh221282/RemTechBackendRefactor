namespace Users.Module.Features.AddUserByAdmin;

internal sealed class RoleNotFoundException : Exception
{
    public RoleNotFoundException(string roleName)
        : base($"Роль {roleName} не найдена.") { }
}
