using Identity.Application.AccountPermissions;
using Identity.Contracts.AccountPermissions;
using Identity.Contracts.AccountPermissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.AccountPermissions.EventListeners.OnRegistered;

public sealed class LoggingOnRegisteredAccountPermissionEventListener(
    Serilog.ILogger logger
    ) : 
    IOnAccountPermissionCreatedEventListener
{
    private readonly Serilog.ILogger _logger = logger.ForContext<IOnAccountPermissionCreatedEventListener>();
    
    public Task<Result<Unit>> React(AccountPermissionData data, CancellationToken ct = default)
    {
        _logger.Information("Account permission has been registered.");
        AccountPermissionLog log = new(new AccountPermission(data));
        log.Log(_logger);
        return Task.FromResult(Result.Success(Unit.Value));
    }
}