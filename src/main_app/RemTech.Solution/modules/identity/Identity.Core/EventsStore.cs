using System.Reflection;
using Identity.Core.Permissions;

namespace Identity.Core;

public sealed class EventsStore
{
    private readonly Dictionary<Type, List<object>> _subscriptions = [];

    public void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : Event
    {
        Type eventType = typeof(TEvent);
        AddEventHandler(eventType, handler);
    }
    
    public void SubscribeByObjectFields(object objectWithHandlers)
    {
        Type objectType = objectWithHandlers.GetType();
        FieldInfo[] fields = objectType.GetFields();
        foreach (var field in fields)
        {
            object? handler = field.GetValue(objectWithHandlers);
            if (handler is null) continue;
            if (!IsEventHandler(handler, out Type[] @interfaces)) continue;
            Subscribe(handler);
        }
    }
    
    
    
    public void Subscribe(object handler)
    {
        Type handlerType = handler.GetType();
        if (!IsEventHandler(handler, out Type[] @interfaces))
            throw new InvalidOperationException($"{handler.GetType().Name} is not an event handler.");

        foreach (var handlerInterface in @interfaces)
        {
            Type eventType = handlerInterface.GetGenericArguments()[0];
            AddEventHandler(eventType, handler);
        }
    }
    
    public void Raise<TEvent>(TEvent @event) where TEvent : Event
    {
        Type eventType = typeof(TEvent);
        if (!_subscriptions.TryGetValue(eventType, out List<object>? subscriptions))
            return;
        
        IEnumerable<IEventHandler<TEvent>> handlers = subscriptions.OfType<IEventHandler<TEvent>>();
        foreach (var handler in handlers)
            handler.ReactOnEvent(@event);
    }

    private bool IsEventHandler(object @object, out Type[] handlers)
    {
        Type handlerType = @object.GetType();
        
        handlers = handlerType
            .GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
            .ToArray();

        return handlers.Length > 0;
    }
    
    private void AddEventHandler(Type eventType, object handler)
    {
        if (!_subscriptions.TryGetValue(eventType, out List<object>? subscriptions))
        {
            subscriptions = [];
            _subscriptions.Add(eventType, subscriptions);
        }
        
        subscriptions.Add(handler);
    }
}