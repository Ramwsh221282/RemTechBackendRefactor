using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Primitives.Extensions;

namespace RemTech.BuildingBlocks.DependencyInjection;

public static class ServiceDelegatesModule
{
    private static readonly Type BaseDelegateType = typeof(Delegate);
    
    extension(IServiceCollection services)
    {
        public void AddScopedDelegate<TDelegate>(Assembly assembly)
        where TDelegate : Delegate
        {
            Type requiredDelegateType = typeof(TDelegate);
            Type[] factories = assembly.TypesWithDelegateFactories(requiredDelegateType);
            MethodInfo[] methods = factories.Map(t => t.GetDelegateFactoryMethods(requiredDelegateType)).SelectMany(ma => ma.Select(m => m)).ToArray();
            services.RegisterScopeds<TDelegate>(methods);
        }
        
        public void AddTransientDelegate<TDelegate>(Assembly assembly)
            where TDelegate : Delegate
        {
            Type requiredDelegateType = typeof(TDelegate);
            Type[] factories = assembly.TypesWithDelegateFactories(requiredDelegateType);
            MethodInfo[] methods = factories.Map(t => t.GetDelegateFactoryMethods(requiredDelegateType)).SelectMany(ma => ma.Select(m => m)).ToArray();
            services.RegisterTransients<TDelegate>(methods);
        }
        
        public void AddSingletonDelegate<TDelegate>(Assembly assembly)
            where TDelegate : Delegate
        {
            Type requiredDelegateType = typeof(TDelegate);
            Type[] factories = assembly.TypesWithDelegateFactories(requiredDelegateType);
            MethodInfo[] methods = factories.Map(t => t.GetDelegateFactoryMethods(requiredDelegateType)).SelectMany(ma => ma.Select(m => m)).ToArray();
            services.RegisterSingletons<TDelegate>(methods);
        }
        
        private void RegisterTransients<TDelegate>(MethodInfo[] methods)
            where TDelegate : Delegate
        {
            methods.ForEach(services.RegisterTransient<TDelegate>);
        }
        
        private void RegisterScopeds<TDelegate>(MethodInfo[] methods)
            where TDelegate : Delegate
        {
            methods.ForEach(services.RegisterScoped<TDelegate>);
        }
        
        private void RegisterSingletons<TDelegate>(MethodInfo[] methods)
            where TDelegate : Delegate
        {
            methods.ForEach(services.RegisterSingleton<TDelegate>);
        }
        
        private void RegisterTransient<TDelegate>(MethodInfo method)
            where TDelegate : Delegate
        {
            services.AddTransient<TDelegate>(sp =>
            {
                object[] dependencies = GetRequiredFactoryDependencies(method, sp);
                TDelegate @delegate = (TDelegate)method.Invoke(null, dependencies)!;
                return @delegate;
            });
        }
        
        private void RegisterScoped<TDelegate>(MethodInfo method)
            where TDelegate : Delegate
        {
            services.AddScoped<TDelegate>(sp =>
            {
                object[] dependencies = GetRequiredFactoryDependencies(method, sp);
                TDelegate @delegate = (TDelegate)method.Invoke(null, dependencies)!;
                return @delegate;
            });
        }
        
        private void RegisterSingleton<TDelegate>(MethodInfo method)
        where TDelegate : Delegate
        {
            services.AddSingleton<TDelegate>(sp =>
            {
                object[] dependencies = GetRequiredFactoryDependencies(method, sp);
                TDelegate @delegate = (TDelegate)method.Invoke(null, dependencies)!;
                return @delegate;
            });
        }
    }

    extension(MethodInfo methodInfo)
    {
        private Type[] GetParameterTypes()
        {
            return methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
        }
    }
    
    extension(Assembly assembly)
    {
        private Type[] TypesWithDelegateFactories(Type requiredDelegateType)
        {
            return assembly.GetTypes().Where(t => t.HasDelegateFactoryMethods(requiredDelegateType)).ToArray();
        }
    }
    
    extension(Type type)
    {
        private MethodInfo[] GetDelegateFactoryMethods(Type delegateType)
        {
            return type.GetMethods().Where(m => MethodIsStaticFactory(m, delegateType)).ToArray();
        }
        
        private bool HasDelegateFactoryMethods(Type delegateType)
        {
            return type.GetMethods().Where(m => MethodIsStaticFactory(m, delegateType)).ToArray().Length > 0;
        }
    }

    private static bool MethodIsStaticFactory(MethodInfo method, Type delegateType)
    {
        return MethodReturnsDelegate(method, delegateType) && MethodIsStatic(method);
    }
    
    private static bool MethodReturnsDelegate(MethodInfo method, Type delegateType)
    {
        return BaseDelegateType.IsAssignableFrom(method.ReturnParameter.ParameterType) && method.ReturnParameter.ParameterType == delegateType;
    }

    private static bool MethodIsStatic(MethodInfo method)
    {
        return method.IsStatic;
    }
    
    private static object[] GetRequiredFactoryDependencies(MethodInfo method, IServiceProvider sp)
    {
        Type[] parameterTypes = method.GetParameterTypes();
        int length = parameterTypes.Length;
        object[] dependencies = new object[length];
        for (int index = 0; index < length; index++)
        {
            object dependency = sp.GetRequiredService(parameterTypes[index]);
            dependencies[index] =  dependency;
        }
        return dependencies;
    }
}