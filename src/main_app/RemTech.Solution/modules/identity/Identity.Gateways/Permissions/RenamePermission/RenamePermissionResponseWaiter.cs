using Identity.Core;
using Identity.Core.Permissions.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Gateways.Permissions.RenamePermission;

public sealed class RenamePermissionResponseWaiter : IEventHandler<PermissionRenamed>
{
    private Result<RenamePermissionResponse> _result =
        Result.Failure<RenamePermissionResponse>(Error.Application("Операция еще не выполнена"));
    
    public void ReactOnEvent(PermissionRenamed @event)
    {
        RenamePermissionResponse response = new(@event.Id, @event.NewName);
        _result = Result.Success(response);
    }

    public Result<RenamePermissionResponse> Read()
    {
        return _result;
    }
}