using Identity.Adapter.Storage.EventHandlers;
using Identity.Domain.Users.Events;

namespace Identity.Adapter.Storage.DependencyInjection;

internal static class StorageAdapterEventHandlers
{
    private static readonly Dictionary<Type, Type> Handlers = InitializeHandlers();

    internal static Type? GetHandlerType(Type eventType) =>
        Handlers.TryGetValue(eventType, out var handlerType) ? handlerType : null;

    private static Dictionary<Type, Type> InitializeHandlers()
    {
        Type handlerInterface = typeof(IIdentityStorageAdapterEventHandler<>);

        var eventTypes = typeof(IdentityUserEvent)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(IdentityUserEvent)))
            .ToList();

        var handlerTypes = typeof(StorageAdapterEventHandler)
            .Assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.IsClass)
            .Where(t =>
                t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface)
            )
            .ToList();

        var result = new Dictionary<Type, Type>();

        foreach (var eventType in eventTypes)
        {
            var handlerType = handlerTypes.FirstOrDefault(t =>
                t.GetInterfaces()
                    .Any(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == handlerInterface
                        && i.GetGenericArguments()[0] == eventType
                    )
            );

            if (handlerType != null)
            {
                var interfaceType = handlerType
                    .GetInterfaces()
                    .First(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == handlerInterface
                        && i.GetGenericArguments()[0] == eventType
                    );

                result[eventType] = interfaceType;
            }
        }

        return result;
    }
}
