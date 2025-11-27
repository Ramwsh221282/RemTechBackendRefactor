using Identity.Application.AccountPermissions;
using Identity.Contracts.AccountPermissions;
using Identity.Contracts.AccountPermissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.AccountPermissions.EventListeners.OnRemoved;

public sealed class LoggingOnAccountPermissionRemovedEventListener(
    Serilog.ILogger logger) :
    IOnAccountPermissionRemovedEventListener
{
    private readonly Serilog.ILogger _logger = logger.ForContext<IOnAccountPermissionRemovedEventListener>();
    
    public Task<Result<Unit>> React(AccountPermissionData data, CancellationToken ct = default)
    {
        _logger.Information("Account permission was removed.");
        AccountPermissionLog log = new(new AccountPermission(data));
        log.Log(_logger);
        return Task.FromResult(Result.Success(Unit.Value));
    }
}