using System.Reflection;
using Mailers.Application.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Mailers.Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddMailersApplication()
        {
            services.RegisterFunctions<CreateMailerFunction>();
            services.RegisterAsyncFunctions<CreateMailerFunction>();
        }
        
        public void RegisterAsyncFunctions<TAssemblyType>()
        {
            var assemblyType = typeof(TAssemblyType);
            var requiredType = typeof(IAsyncFunction<,>);
            services.FindAndRegisterFunctions(assemblyType, requiredType);
        }
        
        public void RegisterFunctions<TAssemblyType>()
        {
            var assemblyType = typeof(TAssemblyType);
            var requiredType = typeof(IFunction<,>);
            services.FindAndRegisterFunctions(assemblyType, requiredType);
        }

        private void FindAndRegisterFunctions(Type assemblyType, Type requiredType)
        {
            var types = FindTypesOf(requiredType, assemblyType.Assembly);
            foreach (var type in types)
            {
                var @interface = FindInterfaceOf(type, requiredType);
                services.AddTransient(@interface, type);
            }
        }
    }
    
    private static Type[] FindTypesOf(Type type, Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type))
            .ToArray();
    }

    private static Type FindInterfaceOf(Type source, Type type)
    {
        return source.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == type)!;
    }
}