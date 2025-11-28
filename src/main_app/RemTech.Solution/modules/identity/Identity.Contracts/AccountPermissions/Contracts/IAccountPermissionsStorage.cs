using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace Identity.Contracts.AccountPermissions.Contracts;

public interface IAccountPermissionsStorage :
    IEntityPersister<IAccountPermission>,
    IEntityFetcher<IAccountPermission, AccountPermissionQueryArgs>,
    IEntityRemover<IAccountPermission>
{
    
}