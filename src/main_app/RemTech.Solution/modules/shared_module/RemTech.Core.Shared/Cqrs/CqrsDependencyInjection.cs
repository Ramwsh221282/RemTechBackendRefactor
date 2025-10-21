using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Core.Shared.Cqrs;

public static class CqrsDependencyInjection
{
    public static void AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        Type genericInterface = typeof(ICommandHandler<,>);

        Type[] classes = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && t.IsClass)
            .Where(t =>
                t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface)
            )
            .ToArray();

        foreach (Type @class in classes)
        {
            Type[] interfaces = @class
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface)
                .ToArray();

            foreach (Type @interface in interfaces)
                services.AddScoped(@interface, @class);
        }
    }
}
