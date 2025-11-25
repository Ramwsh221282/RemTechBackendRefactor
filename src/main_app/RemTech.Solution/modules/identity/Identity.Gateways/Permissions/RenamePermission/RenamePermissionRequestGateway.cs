using Identity.Core;
using Identity.Core.Permissions;
using Identity.Infrastructure.Logging;
using Identity.Infrastructure.NpgSql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace Identity.Gateways.Permissions.RenamePermission;

public sealed class RenamePermissionRequestGateway (
    NpgSqlPermissionsStorage storage,
    PermissionsLogger logger,
    RenamePermissionResponseWaiter waiter,
    EventsStore events) : 
    IGateway<RenamePermissionRequest, RenamePermissionResponse>
{
    private readonly NpgSqlPermissionsStorage _storage = storage;
    private readonly PermissionsLogger _logger = logger;
    private readonly RenamePermissionResponseWaiter _waiter = waiter;
    
    public async Task<Result<RenamePermissionResponse>> Execute(RenamePermissionRequest request)
    {
        return await new AsyncOperation<RenamePermissionResponse>(async () =>
        {
            PermissionsQueryArgs query = new(Id: request.Id, WithLock: true);
            Permission? existing = await _storage.Get(query, request.Ct);
            if (existing is null) throw ErrorException.NotFound("Разрешение не найдено.");
            Permission withEventsStore = existing.AttachEventStore(events);
            withEventsStore.Rename(request.NewName);
            await storage.ProcessDatabaseOperations(request.Ct);
            _logger.ProcessLogging();
            return _waiter.Read();
        }).Process();
    }
}