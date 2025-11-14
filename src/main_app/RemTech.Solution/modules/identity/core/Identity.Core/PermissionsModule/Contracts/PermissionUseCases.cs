namespace Identity.Core.PermissionsModule.Contracts;

public sealed record RegisterPermissionArgs(string Name, CancellationToken Ct);
public delegate Task<Result<Permission>> RegisterPermission(RegisterPermissionArgs args);

public static class PermissionUseCases
{
    public static RegisterPermission DefaultRegisterPermission =>
        args =>
        {
            string name = args.Name;
            Result<Permission> permission = Permission.Create(name);
            return Task.FromResult(permission);
        };
}