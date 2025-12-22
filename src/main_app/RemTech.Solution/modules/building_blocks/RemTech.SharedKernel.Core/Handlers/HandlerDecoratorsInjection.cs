using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace RemTech.SharedKernel.Core.Handlers;

public static class HandlerDecoratorsInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterLoggingHandlers()
        {
            Assembly assembly = typeof(ILoggingCommandHandler<,>).Assembly;
            services.Scan(x => x.FromAssemblies([assembly])
                .AddClasses(classes => classes.AssignableTo(typeof(ILoggingCommandHandler<,>)))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsSelfWithInterfaces()
                .WithScopedLifetime());
        }

        public void RegisterValidatingHandlers()
        {
            Assembly assembly = typeof(IValidatingCommandHandler<,>).Assembly;
            services.Scan(x => x.FromAssemblies([assembly])
                .AddClasses(classes => classes.AssignableTo(typeof(IValidatingCommandHandler<,>)))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsSelfWithInterfaces()
                .WithScopedLifetime());
        }

        public void UseLoggingHandlers()
        {
            services.TryDecorate(typeof(ICommandHandler<,>), typeof(LoggingHandler<,>));
        }

        public void UseValidatingHandlers()
        {
            services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidatingHandler<,>));
        }
    }
}