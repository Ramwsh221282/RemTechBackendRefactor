using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Notifications.Abstractions;

public sealed class IdentitySubjectsEventsRegistry
{
    private readonly Dictionary<Type, List<object>> _subscriptions = [];
    private readonly Queue<AsyncResultFn> _queues = [];

    public IdentitySubjectsEventsRegistry AddHandlerFor<TEvent>(AsyncSubjectNotificationHandle<TEvent> handler) 
        where TEvent : IdentitySubjectNotification
    {
        Type eventType = typeof(TEvent);
        if (!_subscriptions.TryGetValue(eventType, out List<object>? handlers))
        {
            handlers = [];
            _subscriptions[eventType] = handlers;
        }
        
        handlers.Add(handler);
        return this;
    }

    public void Record<TEvent>(TEvent notification) 
        where TEvent : IdentitySubjectNotification
    {
        Type eventType = typeof(TEvent);
        if (!_subscriptions.TryGetValue(eventType, out List<object>? handlers))
            return;

        foreach (AsyncSubjectNotificationHandle<TEvent> handler in handlers.Cast<AsyncSubjectNotificationHandle<TEvent>>())
            _queues.Enqueue(async (ct) => await handler(notification, ct));
    }

    public async Task<Result<Unit>> ProcessEvents(CancellationToken ct = default)
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
}