using Identity.Contracts.Permissions;
using Identity.Contracts.Shared.Contracts;

namespace Identity.Application.Permissions.Contracts;

public interface IPermissionsStorage : 
    IEntityPersister<Permission>,
    IEntityFetcher<Permission, PermissionQueryArgs>,
    IEntityUpdater<Permission>;