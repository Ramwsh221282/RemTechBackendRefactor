using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RemTech.DependencyInjection;

public static class DependencyInjectionExtension
{
    public static T GetService<T>(this IServiceProvider provider)
        where T : class
    {
        return provider.GetRequiredService<T>();
    }

    public static T GetService<T>(this AsyncServiceScope scope)
        where T : class
    {
        return scope.ServiceProvider.GetService<T>();
    }

    public static void RegisterModuleServices(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        string moduleName
    )
    {
        IEnumerable<Assembly> filtered = assemblies.GetFilteredAssemblies(moduleName);
        IEnumerable<Type> types = filtered.GetInjectionClasses();
        IEnumerable<MethodInfo> methods = types.GetInjectionMethods();
        methods.InvokeMethods(services);
    }

    private static IEnumerable<Assembly> GetFilteredAssemblies(
        this IEnumerable<Assembly> assemblies,
        string moduleName
    ) =>
        assemblies.Where(a =>
            !a.IsDynamic && a.FullName != null && a.FullName.StartsWith(moduleName)
        );

    private static IEnumerable<Type> GetInjectionClasses(this IEnumerable<Assembly> assemblies)
    {
        return assemblies
            .SelectMany(a =>
                a.GetTypes().Where(t => t.GetCustomAttribute<InjectionClassAttribute>() != null)
            )
            .Select(t => t);
    }

    private static IEnumerable<MethodInfo> GetInjectionMethods(this IEnumerable<Type> types) =>
        types.SelectMany(t =>
            t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.GetCustomAttribute<InjectionMethodAttribute>() != null)
                .Where(m => m.ReturnType == typeof(void))
                .Where(m =>
                    m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == typeof(IServiceCollection)
                )
        );

    private static void InvokeMethods(
        this IEnumerable<MethodInfo> methods,
        IServiceCollection services
    )
    {
        foreach (MethodInfo method in methods)
            method.Invoke(null, [services]);
    }
}
