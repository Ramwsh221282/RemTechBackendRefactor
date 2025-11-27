using Identity.Contracts.Shared.Contracts;

namespace Identity.Contracts.AccountPermissions.Contracts;

public interface IAccountPermissionsStorage :
    IEntityPersister<IAccountPermission>,
    IEntityFetcher<IAccountPermission, AccountPermissionQueryArgs>,
    IEntityRemover<IAccountPermission>
{
    
}