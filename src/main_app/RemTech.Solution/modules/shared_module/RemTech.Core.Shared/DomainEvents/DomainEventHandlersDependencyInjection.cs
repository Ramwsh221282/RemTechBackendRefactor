using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RemTech.Core.Shared.DomainEvents;

public static class DomainEventHandlersDependencyInjection
{
    public static void AddDomainEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        services.TryAddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();

        IEnumerable<Type> classes = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.GetInterfaces().Any(InterfaceType));

        foreach (Type @class in classes)
        {
            IEnumerable<Type> @interfaces = @class.GetInterfaces().Where(InterfaceType);
            foreach (Type @interface in @interfaces)
                services.AddScoped(@interface, @class);
        }
    }

    private static bool InterfaceType(Type @interface) =>
        @interface.IsGenericType
        && @interface.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>);
}
