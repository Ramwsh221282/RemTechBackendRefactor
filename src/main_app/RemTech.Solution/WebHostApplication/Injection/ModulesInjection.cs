using System.Reflection;
using System.Transactions;
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
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;
using RemTech.SharedKernel.Core.Handlers.Decorators.Logging;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;
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

            services.RegisterHandlers(typeof(IQueryHandler<,>), assemblies);
            services.RegisterHandlers(typeof(ICachingQueryHandler<,>), assemblies);
            services.RegisterHandlers(typeof(IEventTransporter<,>), assemblies);
            services.RegisterHandlers(typeof(ICacheInvalidator<,>), assemblies);
            services.RegisterHandlers(typeof(ICommandHandler<,>), assemblies);

            services.RegisterConsumers(assemblies);
            services.RegisterDomainEventHandlers(assemblies);

            services.UseTransactionalHandlers();
            services.UseValidatingHandlers();
            services.UseCacheInvalidatingHandlers();
            services.UseLoggingCommandHandlers();
        }

        private void UseLoggingCommandHandlers() =>
            services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingHandler<,>));

        private void UseTransactionalHandlers() =>
            services.Decorate(typeof(ICommandHandler<,>), typeof(TransactionalHandler<,>));

        private void UseValidatingHandlers() =>
            services.Decorate(typeof(ICommandHandler<,>), typeof(ValidatingHandler<,>));

        private void UseCacheInvalidatingHandlers() =>
            services.Decorate(typeof(ICommandHandler<,>), typeof(CacheInvalidatingHandler<,>));

        private void RegisterDomainEventHandlers(Assembly[] assemblies) =>
            services.Scan(x =>
                x.FromAssemblies(assemblies)
                    .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                    .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
            );

        private void RegisterHandlers(Type handlerType, Assembly[] assemblies) =>
            services.Scan(x =>
                x.FromAssemblies(assemblies)
                    .AddClasses(classes => classes.AssignableTo(handlerType))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
            );

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
