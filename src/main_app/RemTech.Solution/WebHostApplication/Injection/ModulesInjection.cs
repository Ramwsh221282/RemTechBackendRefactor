using System.Reflection;
using ContainedItems.Domain.Models;
using ContainedItems.Infrastructure.Repositories;
using ContainedItems.Worker.Extensions;
using FluentMigrator.Runner;
using Identity.Domain.Accounts.Models;
using Identity.Infrastructure.Accounts;
using Identity.WebApi.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTech.SharedKernel.Infrastructure.Redis;
using RemTech.SharedKernel.NN;
using Scrutor;
using Spares.Domain.Models;
using Spares.Infrastructure.Repository;
using Spares.WebApi.Extensions;
using Vehicles.Domain.Vehicles;
using Vehicles.Infrastructure.BackgroundServices;
using Vehicles.Infrastructure.Vehicles.PersisterImplementation;
using Vehicles.WebApi.Extensions;

namespace WebHostApplication.Injection;

public static class ModulesInjection
{
	extension(IServiceCollection services)
	{
		public void RegisterSharedDependencies(IConfigurationManager configuration)
		{
			services.RegisterLogging();
			services.AddPostgres();
			services.AddRabbitMq();
			services.RegisterHybridCache(configuration);
			RemTech.SharedKernel.Infrastructure.AesEncryption.AesCryptographyExtensions.AddAesCryptography(services);
			services.TryAddSingleton<EmbeddingsProvider>();
		}

		public void RegisterModuleMigrations()
		{
			Assembly[] assemblies = GetModulesAssemblies();
			services.AddMigrations(assemblies);
		}

		public void RegisterApplicationModules()
		{
			Assembly[] assemblies = GetModulesAssemblies();
			services.RegisterInfrastructureDependencies();

			services.RegisterHandlers(typeof(IQueryHandler<,>), assemblies);

			services.RegisterHandlers(typeof(IQueryExecutorWithCache<,>), assemblies);
			services.RegisterHandlers(typeof(IEventTransporter<,>), assemblies);
			services.RegisterHandlers(typeof(ICacheInvalidator<,>), assemblies);
			services.RegisterHandlers(typeof(ICommandHandler<,>), assemblies);

			services.RegisterConsumers(assemblies);
			services.RegisterDomainEventHandlers(assemblies);

			services.DecorateCommandHandlersWith(typeof(TransactionalHandler<,>));
			services.DecorateCommandHandlersWith(typeof(ValidatingHandler<,>));
			services.DecorateCommandHandlersWith(typeof(CacheInvalidatingHandler<,>));
			services.DecorateCommandHandlersWith(typeof(LoggingCommandHandler<,>));

			services.DecorateQueryHandlersWith(typeof(TestCachingQueryHandler<,>));
			services.DecorateQueryHandlersWith(typeof(TestLoggingQueryHandler<,>));
		}

		private void DecorateCommandHandlersWith(Type decoratorType)
		{
			Type commandHandlerType = typeof(ICommandHandler<,>);
			services.TryDecorate(commandHandlerType, decoratorType);
		}

		private void DecorateQueryHandlersWith(Type type)
		{
			Type queryHandlerType = typeof(IQueryHandler<,>);
			services.TryDecorate(queryHandlerType, type);
		}

		private void RegisterDomainEventHandlers(Assembly[] assemblies) =>
			services.Scan(x =>
				x.FromAssemblies(assemblies)
					.AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
					.UsingRegistrationStrategy(RegistrationStrategy.Skip)
					.AsSelfWithInterfaces()
					.WithScopedLifetime()
					.AddClasses(classes => classes.AssignableTo<IDomainEventHandler>())
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
			typeof(ContainedItem).Assembly,
			typeof(ContainedItemsRepository).Assembly,
			// parsers module
			typeof(SubscribedParser).Assembly,
			typeof(SubscribedParsersRepository).Assembly,
			typeof(VehicleEmbeddingsUpdaterService).Assembly,
		];
}

public interface ITestCachingQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
	where TQuery : IQuery;
