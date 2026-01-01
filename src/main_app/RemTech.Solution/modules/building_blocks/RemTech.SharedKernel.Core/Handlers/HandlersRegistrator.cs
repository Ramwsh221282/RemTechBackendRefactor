using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace RemTech.SharedKernel.Core.Handlers;

public sealed class HandlersRegistrator
{
    private readonly IServiceCollection _services;
    private Assembly[] _assemblies = [];
    private readonly Queue<Action> _registrationActions = new();
    public HandlersRegistrator(IServiceCollection services) => _services = services;

    public HandlersRegistrator FromAssemblies(Assembly[] assemblies)
    {
        _assemblies = [..assemblies];
        return this;
    }

    public HandlersRegistrator RequireRegistrationOf(Type type)
    {
        _registrationActions.Enqueue(() => _services.Scan(x => x.FromAssemblies(_assemblies)
            .AddClasses(classes => classes.AssignableTo(type))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelfWithInterfaces()
            .WithScopedLifetime()
        ));
        return this;
    }

    public HandlersRegistrator AlsoAddDecorators()
    {
        _registrationActions.Enqueue(() =>
        {
            _services.RegisterLoggingHandlers();
            _services.RegisterValidatingHandlers();
            _services.RegisterTransactionalHandlers();
        });
        return this;
    }

    public HandlersRegistrator AlsoAddValidators()
    {
        foreach (Assembly assembly in _assemblies)
            _registrationActions.Enqueue(() => _services.AddValidatorsFromAssembly(assembly));
        return this;
    }

    public HandlersRegistrator AlsoUseDecorators()
    {
        _registrationActions.Enqueue(() =>
        {
            _services.UseTransactionalHandlers();
            _services.UseValidatingHandlers();
            _services.UseLoggingHandlers();
        });
        return this;
    }

    public void Invoke()
    {
        while (_registrationActions.Count > 0)
        {
            Action action = _registrationActions.Dequeue();
            action();
        }
    }
}