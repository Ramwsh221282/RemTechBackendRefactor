namespace Identity.Domain.Users.Events;

public sealed record IdentityUserRoleEventArgs(Guid RoleId, string RoleName);