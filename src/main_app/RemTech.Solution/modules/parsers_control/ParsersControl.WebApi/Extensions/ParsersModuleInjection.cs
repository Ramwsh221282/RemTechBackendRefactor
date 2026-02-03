using ParsersControl.Core.Contracts;
using ParsersControl.Infrastructure.Listeners;
using ParsersControl.Infrastructure.Migrations;
using ParsersControl.Infrastructure.Parsers.Repository;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.WebApi.Extensions;

/// <summary>
/// Инъекция зависимостей модуля управления парсерами.
/// </summary>
public static class ParsersModuleInjection
{
	extension(IServiceCollection services)
	{
		public void InjectParsersControlModule()
		{
			services.AddInfrastructureLayer();
		}

		public void AddParsersControlModule(bool isDevelopment)
		{
			services.AddSharedInfrastructure(isDevelopment);
			services.AddInfrastructureLayer();
		}

		private void AddSharedInfrastructure(bool isDevelopment)
		{
			services.RegisterLogging();
			if (isDevelopment)
			{
				services.AddMigrations([typeof(ParsersTableMigration).Assembly]);
				services.AddOptions<NpgSqlOptions>().BindConfiguration(nameof(NpgSqlOptions));
				services.AddOptions<RabbitMqOptions>().BindConfiguration(nameof(RabbitMqOptions));
			}

			services.AddPostgres();
			services.AddRabbitMq();
		}

		public void AddInfrastructureLayer()
		{
			services.AddEventListeners();
			services.AddRepositories();
		}

		private void AddEventListeners()
		{
			services.AddScoped<IOnParserSubscribedListener, OnParserSubscribedEventListener>();
			services.AddScoped<IOnParserStartedListener, OnParserStartedEventListener>();
		}

		private void AddRepositories()
		{
			services.AddScoped<ISubscribedParsersRepository, SubscribedParsersRepository>();
			services.AddScoped<ISubscribedParsersCollectionRepository, SubscribedParsersCollectionRepository>();
		}
	}
}
