using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.Handlers;

namespace Spares.Domain;

public static class SparesDomainDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddSparesDomain()
        {
            services.RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            new HandlersRegistrator(services)
                .FromAssemblies([typeof(SparesDomainDependencyInjection).Assembly])
                .RequireRegistrationOf(typeof(ICommandHandler<,>))
                .AlsoAddValidators()
                .AlsoAddDecorators()
                .AlsoUseDecorators()
                .Invoke();
        }
    }
}