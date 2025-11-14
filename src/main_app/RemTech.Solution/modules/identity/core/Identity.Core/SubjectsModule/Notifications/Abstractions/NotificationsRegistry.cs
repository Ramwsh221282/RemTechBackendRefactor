namespace Identity.Core.SubjectsModule.Notifications.Abstractions;

public sealed class NotificationsRegistry
{
    private readonly Dictionary<Type, List<object>> _subscriptions = [];
    private readonly Queue<AsyncResultFn> _queues = [];
    
    public void AddNotificationHandlers<TEvent>(IEnumerable<AsyncNotificationHandle<TEvent>> fns) 
        where TEvent : Notification
    {
        NotificationsRegistry registry = this;
        foreach (AsyncNotificationHandle<TEvent> fn in fns)
            registry.AddNotificationHandler(fn);
    }
    
    public void AddNotificationHandler<TEvent>(AsyncNotificationHandle<TEvent> handler) 
        where TEvent : Notification
    {
        Type eventType = typeof(TEvent);
        if (!_subscriptions.TryGetValue(eventType, out List<object>? handlers))
        {
            handlers = [];
            _subscriptions[eventType] = handlers;
        }
        
        handlers.Add(handler);
    }

    public void Record<TEvent>(TEvent notification) 
        where TEvent : Notification
    {
        Type eventType = typeof(TEvent);
        if (!_subscriptions.TryGetValue(eventType, out List<object>? handlers))
            return;

        foreach (AsyncNotificationHandle<TEvent> handler in handlers.Cast<AsyncNotificationHandle<TEvent>>())
            _queues.Enqueue(async ct => await handler(notification, ct));
    }

    public async Task<Result<Unit>> ProcessNotifications(CancellationToken ct = default)
    {
        while (_queues.Count > 0)
        {
            AsyncResultFn function = _queues.Dequeue();
            Result result = await function(ct);
            if (result.IsFailure)
                return result.Error;
        }

        return Unit.Value;
    }

    public async Task<Result<T>> OnSuccessProcession<T>(Func<T> function, CancellationToken ct = default)
    {
        Result<Unit> processing = await ProcessNotifications(ct);
        return processing.IsFailure ? processing.Error : function();
    }
}