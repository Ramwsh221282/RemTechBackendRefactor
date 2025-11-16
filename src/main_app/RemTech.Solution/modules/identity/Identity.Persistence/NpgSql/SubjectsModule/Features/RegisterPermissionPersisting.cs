using Identity.Core.PermissionsModule;
using Identity.Core.PermissionsModule.Contracts;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.NpgSql.SubjectsModule.Features;

public static class RegisterPermissionPersisting
{
    internal static RegisterPermission RegisterPermissionUseCase(
        RegisterPermission origin,
        PermissionsStorage storage) =>
        async args =>
        {
            Result<Permission> permission = await origin(args);
            if (permission.IsFailure) return permission.Error;

            PermissionSnapshot snapshot = permission.Value.Snapshot();
            if (!await PermissionNameIsUnique(snapshot, storage, args.Ct))
                return Error.Conflict($"Разрешение: {snapshot.Name} уже существует");

            Result<Unit> inserting = await storage.Insert(permission, args.Ct);
            return inserting.IsFailure ? inserting.Error : permission;
        };

    private static async Task<bool> PermissionNameIsUnique(
        PermissionSnapshot snapshot, 
        PermissionsStorage storage, 
        CancellationToken ct)
    {
        PermissionsQueryArgs args = new(Name: snapshot.Name);
        Optional<Permission> withEmail = await storage.Find(args, ct);
        if (withEmail.HasValue) return false;
        return true;
    }

    extension(RegisterPermission origin)
    {
        public RegisterPermission WithPersisting(IServiceProvider sp)
        {
            PermissionsStorage storage = sp.Resolve<PermissionsStorage>();
            return RegisterPermissionUseCase(origin, storage);
        }
    }
}