using System.Reflection;
using Identity.Domain.Users.Events;
using Identity.Domain.Users.Ports.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Result;

namespace Identity.Adapter.Storage.EventHandlers;

public static class EventHandlerExtensions
{
    public static async Task<Status> HandleFor<T>(
        this T @event,
        Type @eventHandler,
        IServiceProvider serviceProvider,
        CancellationToken ct = default
    )
        where T : notnull
    {
        Type handlerType = @event.GetEventHandlerType(@eventHandler);
        MethodInfo method = handlerType.GetEventHandlerMethod();
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        object handler = scope.GetEventHandlerImplementation(handlerType);
        return await method.InvokeMethod(handler, @event, ct);
    }

    private static async Task<Status> InvokeMethod<T>(
        this MethodInfo method,
        object handler,
        T @event,
        CancellationToken ct
    )
        where T : notnull
    {
        Task<Status> task = (Task<Status>)method.Invoke(handler, [@event, ct])!;
        return await task;
    }

    private static MethodInfo GetEventHandlerMethod(this Type eventHandlerType)
    {
        MethodInfo? method = eventHandlerType.GetMethod("Handle");
        if (method == null)
            throw new ApplicationException("Not event handler method");
        return method;
    }

    private static Type GetEventHandlerType<TEvent>(this TEvent @event, Type handlerType)
        where TEvent : notnull
    {
        Type eventType = @event.GetType();
        Type eventHandlerType = handlerType.MakeGenericType(eventType);
        return eventHandlerType;
    }

    private static object GetEventHandlerImplementation(
        this AsyncServiceScope scope,
        Type eventHandlerType
    )
    {
        object? implementation = scope.ServiceProvider.GetService(eventHandlerType);
        return implementation
            ?? throw new ApplicationException(
                $"Implementation for: {eventHandlerType.Name} not found."
            );
    }
}

public sealed class StorageAdapterEventHandler(IServiceProvider serviceProvider)
    : IIdentityUserEventHandler
{
    public async Task<Status> Handle(
        IEnumerable<IdentityUserEvent> events,
        CancellationToken ct = default
    )
    {
        foreach (IdentityUserEvent @event in events)
        {
            Status status = await @event.HandleFor(
                typeof(IIdentityStorageAdapterEventHandler<>),
                serviceProvider,
                ct
            );

            if (status.IsFailure)
                return status;
        }

        return Status.Success();
    }
}
