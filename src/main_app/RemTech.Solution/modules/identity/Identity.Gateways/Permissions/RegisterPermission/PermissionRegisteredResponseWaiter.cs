using Identity.Core;
using Identity.Core.Permissions.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Gateways.Permissions.RegisterPermission;

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