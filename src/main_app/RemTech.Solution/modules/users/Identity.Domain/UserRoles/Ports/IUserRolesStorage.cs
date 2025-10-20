namespace Identity.Domain.UserRoles.Ports;

public interface IUserRolesStorage
{
    Task Add(UserRole role, CancellationToken ct = default);
}
