using Identity.Core;
using Identity.Core.Permissions;
using Identity.Core.Permissions.Events;
using Identity.Infrastructure.Logging;
using Identity.Infrastructure.NpgSql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.RegisterPermission;

public sealed record RegisterPermissionRequest(string Name, CancellationToken Ct) : IRequest;

public sealed record RegisterPermissionResponse(Guid Id, string Name) : IResponse;

public sealed class PermissionRegisteredResponseWaiter : IEventHandler<PermissionRegistered>
{
    private Result<RegisterPermissionResponse> _result =
        Result.Failure<RegisterPermissionResponse>(Error.Conflict("Операция не выполнена."));

    public void ReactOnEvent(PermissionRegistered @event)
    {
        _result = Result.Success(new RegisterPermissionResponse(@event.Id, @event.Name));
    }

    public Result<RegisterPermissionResponse> ReadResult()
    {
        return _result;
    }
}

public sealed class LoggingAsyncOperationResult(
    string operationName,
    Serilog.ILogger logger,
    IAsyncOperationResult result) : IAsyncOperationResult
{
    public async Task<Result<Unit>> Process()
    {
        logger.Information("Processing operation: {name}", operationName);
        Result<Unit> res = await result.Process();
        logger.Information("Completed operation: {name}", operationName);
        if (res.IsFailure) logger.Error("{Error}", res.Error);
        return res;
    }
}

public sealed class LoggingAsyncOperationResult<T>(
    string operationName,
    Serilog.ILogger logger,
    IAsyncOperationResult<T> result) : IAsyncOperationResult<T>
{
    public async Task<Result<T>> Process()
    {
        logger.Information("Processing operation: {name}", operationName);
        Result<T> res = await result.Process();
        logger.Information("Completed operation: {name}", operationName);
        if (res.IsFailure) logger.Error("{Error}", res.Error);
        return res;
    }
}

public sealed class EventSubscriptionStartGateway<TRequest, TResponse>(
    IGateway<TRequest, TResponse> origin,
    EventsStore events)
    : IGateway<TRequest, TResponse>
    where TRequest : IRequest
    where TResponse : IResponse
{
    public async Task<Result<TResponse>> Execute(TRequest request)
    {
        events.SubscribeByObjectFields(origin);
        return await origin.Execute(request);
    }
}

public sealed class RegisterPermissionGateway(
    NpgSqlPermissionsStorage storage,
    PermissionsLogger logger,
    PermissionRegisteredResponseWaiter responseWaiter,
    EventsStore events)
    : IGateway<RegisterPermissionRequest, RegisterPermissionResponse>
{
    private readonly NpgSqlPermissionsStorage _storage = storage;
    private readonly PermissionsLogger _logger = logger;
    private readonly PermissionRegisteredResponseWaiter _waiter = responseWaiter;
    private readonly EventsStore _events = events;
    
    public async Task<Result<RegisterPermissionResponse>> Execute(RegisterPermissionRequest request)
    {
        return await new AsyncOperationResult<RegisterPermissionResponse>(async () =>
        {
            Permission permission = new(Guid.NewGuid(), request.Name, _events);
            permission.Register();
            await _storage.ProcessDatabaseOperations(request.Ct);
            _logger.ProcessLogging();
            return _waiter.ReadResult();
        }).Process();
    }
}