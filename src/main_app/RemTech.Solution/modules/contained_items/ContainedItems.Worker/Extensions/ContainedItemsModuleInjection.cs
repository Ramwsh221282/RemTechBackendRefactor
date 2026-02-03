using ContainedItems.Infrastructure;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ContainedItems.Worker.Extensions;

/// <summary>
/// Расширения для регистрации модуля содержащихся элементов в контейнере служб.
/// </summary>
public static class ContainedItemsModuleInjection
{
	extension(IServiceCollection services)
	{
		public void InjectContainedItemsModule()
		{
			services.RegisterInfrastructure();
		}

		public void AddContainedItemsModule(bool isDevelopment)
		{
			services.RegisterSharedDependencies(isDevelopment);
			services.RegisterInfrastructure();
		}

		public void RegisterInfrastructure()
		{
			services.AddContainedItemsInfrastructure();
		}

		private void RegisterSharedDependencies(bool isDevelopment)
		{
			if (isDevelopment)
			{
				services.AddMigrations([typeof(ContainedItemsInfrastructureInjection).Assembly]);
				services.AddOptions<NpgSqlOptions>().BindConfiguration(nameof(NpgSqlOptions));
				services.AddOptions<RabbitMqOptions>().BindConfiguration(nameof(RabbitMqOptions));
			}

			services.RegisterLogging();
			services.AddPostgres();
			services.AddRabbitMq();
		}
	}
}
