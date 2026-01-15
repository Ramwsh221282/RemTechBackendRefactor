using System.Reflection;
using ContainedItems.Infrastructure.Repositories;
using ContainedItems.Worker.Extensions;
using Identity.Domain.Accounts.Models;
using Identity.Infrastructure.Accounts;
using Identity.WebApi.Extensions;
using Notifications.Core.Mailers;
using Notifications.Infrastructure.Mailers;
using Notifications.WebApi.Extensions;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.Repository;
using ParsersControl.WebApi.Extensions;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Scrutor;
using Spares.Domain.Models;
using Spares.Infrastructure.Repository;
using Spares.WebApi.Extensions;
using Vehicles.Domain.Vehicles;
using Vehicles.Infrastructure.Vehicles.PersisterImplementation;
using Vehicles.WebApi.Extensions;

namespace WebHostApplication.Injection;

public static class ModulesInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterApplicationModules()
        {
            Assembly[] assemblies = GetModulesAssemblies();
            services.RegisterInfrastructureDependencies();

            // first register query handlers:
            services.RegisterQueryHandlers(assemblies);

            // there go dependencies for decorator handlers.
            services.RegisterEventTransporters(assemblies);
            services.RegisterCacheInvalidators(assemblies);

            // there go command handlers
            services.RegisterCommandHandlers(assemblies);
            services.RegisterConsumers(assemblies);

            services.UseCachingQueryHandlers();
        }

        private void UseCachingQueryHandlers()
        {
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(CachedQueryExecutor<,>));
        }

        private void RegisterCommandHandlers(Assembly[] assemblies) =>
            new HandlersRegistrator(services)
                .FromAssemblies(assemblies)
                .RequireRegistrationOf(typeof(ICommandHandler<,>))
                .AlsoAddDomainEventHandlers()
                .AlsoAddDecorators()
                .AlsoAddValidators()
                .AlsoUseDecorators()
                .Invoke();

        private void RegisterQueryHandlers(Assembly[] assemblies)
        {
            services.Scan(x =>
                x.FromAssemblies(assemblies)
                    .AddClasses(classes => classes.AssignableTo(typeof(ICachingQueryHandler<,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
            );

            services.Scan(x =>
                x.FromAssemblies(assemblies)
                    .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
            );
        }

        private void RegisterEventTransporters(Assembly[] assemblies) =>
            new HandlersRegistrator(services)
                .FromAssemblies(assemblies)
                .RequireRegistrationOf(typeof(IEventTransporter<,>))
                .Invoke();

        private void RegisterCacheInvalidators(Assembly[] assemblies) =>
            new HandlersRegistrator(services)
                .FromAssemblies(assemblies)
                .RequireRegistrationOf(typeof(ICacheInvalidator<,>))
                .Invoke();

        private void RegisterConsumers(Assembly[] assemblies)
        {
            services.Scan(x =>
                x.FromAssemblies(assemblies)
                    .AddClasses(classes => classes.AssignableTo<IConsumer>())
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
            );
            services.AddHostedService<AggregatedConsumersHostedService>();
        }

        private void RegisterInfrastructureDependencies()
        {
            ParsersModuleInjection.AddInfrastructureLayer(services);
            NotificationsModuleInjection.AddInfrastructureLayer(services);
            IdentityModuleInjection.AddInfrastructure(services);
            ContainedItemsModuleInjection.RegisterInfrastructure(services);
            SparesModuleInjection.RegisterSparesInfrastructure(services);
            VehiclesModuleInjection.RegisterInfrastructureLayerDependencies(services);
        }
    }

    private static Assembly[] GetModulesAssemblies() =>
        [
            // spares module
            typeof(Spare).Assembly,
            typeof(SparesRepository).Assembly,
            // identity module
            typeof(Account).Assembly,
            typeof(AccountsRepository).Assembly,
            // notifications module
            typeof(Mailer).Assembly,
            typeof(MailersRepository).Assembly,
            // vehicles module
            typeof(Vehicle).Assembly,
            typeof(NpgSqlVehiclesPersister).Assembly,
            // contained items module
            typeof(ContainedItemId).Assembly,
            typeof(ContainedItemsRepository).Assembly,
            // parsers module
            typeof(SubscribedParser).Assembly,
            typeof(SubscribedParsersRepository).Assembly,
        ];
}
