using Identity.Core;
using Identity.Core.Permissions.Events;

namespace Identity.Infrastructure.Logging;

public sealed class PermissionsLogger(Serilog.ILogger logger) : 
    IEventHandler<PermissionRegistered>, 
    IEventHandler<PermissionRenamed>
{
    private readonly Queue<Action<Serilog.ILogger>> _actions = [];

    public void ProcessLogging()
    {
        while (_actions.Count > 0)
        {
            Action<Serilog.ILogger> action = _actions.Dequeue();
            action(logger);
        }
    }
    
    public void ReactOnEvent(PermissionRegistered @event)
    {
        object[] parameters = [@event.Id, @event.Name];
        _actions.Enqueue(l => l.Information("Создано разрешение: {Id} {Name}", parameters));
    }

    public void ReactOnEvent(PermissionRenamed @event)
    {
        object[] parameters = [@event.Id, @event.OldName, @event.NewName];
        _actions.Enqueue(l => l.Information(
            "Разрешение {Id}  переименовано. Старое название: {OldName} Новое название: {NewName}",
            parameters
            ));
    }
}