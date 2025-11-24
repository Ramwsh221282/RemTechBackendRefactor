using Identity.Core.Permissions;

namespace Identity.Core;

public interface IEventHandler<in TEvent> where TEvent : Event
{
    void ReactOnEvent(TEvent @event);
}