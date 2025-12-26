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
            services.AddScoped(typeof(ILoggingCommandHandler<,>), typeof(LoggingHandler<,>));
        }

        public void RegisterValidatingHandlers()
        {
            services.AddScoped(typeof(IValidatingCommandHandler<,>), typeof(ValidatingHandler<,>));
        }
        
        public void RegisterTransactionalHandlers()
        {
            services.AddScoped(typeof(ITransactionalCommandHandler<,>), typeof(TransactionalHandler<,>));
        }
        
        public void UseTransactionalHandlers()
        {
            services.TryDecorate(typeof(ICommandHandler<,>), typeof(TransactionalHandler<,>));
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