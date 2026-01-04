using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts;
using Identity.Domain.Permissions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.GivePermissions;

public sealed class GivePermissionsHandler : ICommandHandler<GivePermissionsCommand, Account>
{
    private IAccountsRepository Accounts { get; }
    private IPermissionsRepository Permissions { get; }
    private IAccountsModuleUnitOfWork UnitOfWork { get; }

    public GivePermissionsHandler(
        IAccountsRepository accounts, 
        IPermissionsRepository permissions,
        IAccountsModuleUnitOfWork unitOfWork)
    {
        Accounts = accounts;
        Permissions = permissions;
        UnitOfWork = unitOfWork;
    }
    
    public async Task<Result<Account>> Execute(GivePermissionsCommand command, CancellationToken ct = new CancellationToken())
    {
        Result<Account> account = await GetRequiredAccount(command, ct);
        if (account.IsFailure) return account.Error;
        
        IEnumerable<Permission> permissions = await GetRequiredPermissions(command, ct);
        if (!PermissionsFound(permissions, command, out Error error)) return error;
        
        Result<Unit> add = account.Value.AddPermissions(permissions);
        if (add.IsFailure) return add.Error;
        
        await UnitOfWork.Save(account.Value, ct);
        return account.Value;
    }

    private bool PermissionsFound(IEnumerable<Permission> permissions, GivePermissionsCommand command, out Error error)
    {
        error = Error.NoError();
        IEnumerable<Guid> notFoundPermissions = command.Permissions.Select(p => p.Id).ExceptBy(permissions.Select(fp => fp.Id.Value), fp => fp);
        if (notFoundPermissions.Any())
        {
            string message = $"Разрешения не найдены: {string.Join(", ", notFoundPermissions.ToString())}";
            error = Error.NotFound(message);
            return false;
        }
        return true;
    }
    
    private async Task<IEnumerable<Permission>> GetRequiredPermissions(GivePermissionsCommand command, CancellationToken ct)
    {
        IEnumerable<PermissionSpecification> specs = command.Permissions.Select(p => new PermissionSpecification().WithId(p.Id).WithLock());
        return await Permissions.GetMany(specs, ct);
    }

    private async Task<Result<Account>> GetRequiredAccount(GivePermissionsCommand command, CancellationToken ct)
    {
        AccountSpecification spec = new AccountSpecification().WithId(command.AccountId).WithLock();
        return await Accounts.Get(spec, ct);
    }
}