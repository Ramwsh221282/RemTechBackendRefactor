using ContainedItems.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.Handlers;

namespace ContainedItems.Domain;

public static class ContainedItemsDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddContainedItemsModule()
        {
            services.AddHandlers();
        }

        private void AddHandlers() => 
            new HandlersRegistrator(services).FromAssembly(typeof(ContainedItem).Assembly)
            .RequireRegistrationOf(typeof(ICommandHandler<,>))
            .AlsoAddValidators()
            .AlsoAddDecorators()
            .AlsoUseDecorators()
            .Invoke();
    }
}