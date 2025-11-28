using Identity.Contracts.Permissions;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace Identity.Application.Permissions.Contracts;

public interface IPermissionsStorage : 
    IEntityPersister<Permission>,
    IEntityFetcher<Permission, PermissionQueryArgs>,
    IEntityUpdater<Permission>;