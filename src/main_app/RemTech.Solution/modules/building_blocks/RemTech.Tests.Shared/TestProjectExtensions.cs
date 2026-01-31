using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Infrastructure.Quartz;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace RemTech.Tests.Shared;

/// <summary>
///  Расширения для настройки тестового проекта.
/// </summary>
public static class TestProjectExtensions
{
	/// <summary>
	/// Пере-регистрирует все задания, реализующие ICronScheduleJob, в контейнере служб.
	/// </summary>
	/// <param name="services">Контейнер служб для перерегистрации заданий.</param>
	public static void ReRegisterCronScheduleJobs(this IServiceCollection services)
	{
		IEnumerable<ServiceDescriptor> jobs = services.Where(s => s.ServiceType == typeof(ICronScheduleJob));
		services.RemoveAll<ICronScheduleJob>();
		foreach (ServiceDescriptor? job in jobs)
		{
			if (job.ImplementationType is null)
				continue;
			services.AddTransient(job.ServiceType, job.ImplementationType);
		}
	}

	/// <summary>
	/// Пере-регистрирует QuartzHostedService в контейнере служб.
	/// </summary>
	/// <param name="services">Контейнер служб для перерегистрации QuartzHostedService.</param>
	/// <param name="configure">Опции для настройки QuartzHostedService.</param>
	public static void ReRegisterQuartzHostedService(
		this IServiceCollection services,
		Action<QuartzHostedServiceOptions>? configure = null
	)
	{
		ServiceDescriptor? quartzHostedService = services.FirstOrDefault(s =>
			s.ServiceType == typeof(IHostedService) && s.ImplementationType == typeof(QuartzHostedService)
		);

		if (quartzHostedService != null)
			services.Remove(quartzHostedService);

		services.AddQuartzHostedService(configure);
	}

	/// <summary>
	/// Пере-регистрирует IConfigurationRoot из файла appsettings.json в контейнере служб.
	/// </summary>
	/// <param name="services">Контейнер служб для перерегистрации IConfigurationRoot.</param>
	public static void ReRegisterAppsettingsJsonConfiguration(this IServiceCollection services)
	{
		IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
		services.RemoveAll<IConfigurationRoot>();
		services.AddSingleton(configurationRoot);
	}

	/// <summary>
	/// Пере-регистрирует NpgSqlOptions в контейнере служб.
	/// </summary>
	/// <param name="services">Контейнер служб для перерегистрации NpgSqlOptions.</param>
	/// <param name="container">Контейнер PostgreSQL для создания конфигурации базы данных.</param>
	public static void ReRegisterNpgSqlOptions(this IServiceCollection services, PostgreSqlContainer container)
	{
		services.RemoveAll<IOptions<NpgSqlOptions>>();
		IOptions<NpgSqlOptions> options = Options.Create(container.CreateDatabaseConfiguration());
		services.AddSingleton(options);
	}

	/// <summary>
	/// Пере-регистрирует RabbitMqOptions в контейнере служб.
	/// </summary>
	/// <param name="services">Контейнер служб для перерегистрации RabbitMqOptions.</param>
	/// <param name="container">Контейнер RabbitMQ для создания конфигурации RabbitMQ.</param>
	public static void ReRegisterRabbitMqOptions(this IServiceCollection services, RabbitMqContainer container)
	{
		services.RemoveAll<IOptions<RabbitMqOptions>>();
		IOptions<RabbitMqOptions> options = Options.Create(container.CreateRabbitMqConfiguration());
		services.AddSingleton(options);
	}

	/// <summary>
	/// Пере-регистрирует фоновую службу в контейнере служб.
	/// </summary>
	/// <typeparam name="T">Тип фоновой службы для перерегистрации.</typeparam>
	/// <param name="services">Контейнер служб для перерегистрации фоновой службы.</param>
	/// <exception cref="InvalidOperationException">Исключение выбрасывается, если служба для перерегистрации не найдена.</exception>
	public static void ReRegisterBackgroundService<T>(this IServiceCollection services)
		where T : class, IHostedService
	{
		ServiceDescriptor? requiredToRemove = services.FirstOrDefault(s => s.ImplementationType == typeof(T));
		if (requiredToRemove is null)
		{
			throw new InvalidOperationException(
				$"Cannot find registered service for {typeof(T)} when re registering background service."
			);
		}

		services.Remove(requiredToRemove);
		services.AddHostedService<T>();
	}

	/// <summary>
	/// Строит и настраивает контейнер PostgreSQL с расширением pgvector для использования в тестах.
	/// </summary>
	/// <param name="builder">Построитель контейнера PostgreSQL.</param>
	/// <returns>Настроенный контейнер PostgreSQL с расширением pgvector.</returns>
	public static PostgreSqlContainer BuildPgVectorContainer(this PostgreSqlBuilder builder) =>
		builder
			.WithImage("pgvector/pgvector:0.8.0-pg17-bookworm")
			.WithDatabase("database")
			.WithUsername("username")
			.WithPassword("password")
			.Build();

	/// <summary>
	/// Строит и настраивает контейнер RabbitMQ для использования в тестах.
	/// </summary>
	/// <param name="builder">Построитель контейнера RabbitMQ.</param>
	/// <returns>Настроенный контейнер RabbitMQ.</returns>
	public static RabbitMqContainer BuildRabbitMqContainer(this RabbitMqBuilder builder) =>
		builder.WithImage("rabbitmq:3.11").Build();

	/// <summary>
	/// Строит и настраивает контейнер Redis для использования в тестах.
	/// </summary>
	/// <param name="builder">Построитель контейнера Redis.</param>
	/// <returns>Настроенный контейнер Redis.</returns>
	public static RedisContainer BuildRedisContainer(this RedisBuilder builder) =>
		builder.WithImage("redis:alpine").Build();

	/// <summary>
	/// Получает строку подключения из контейнера Redis.
	/// </summary>
	/// <param name="container">Контейнер Redis.</param>
	/// <returns>Строка подключения к Redis.</returns>
	public static string GetConnectionString(this RedisContainer container) => container.GetConnectionString();

	/// <summary>
	/// Создает конфигурацию базы данных NpgSqlOptions из контейнера PostgreSQL.
	/// </summary>
	/// <param name="container">Контейнер PostgreSQL.</param>
	/// <returns>Конфигурация базы данных NpgSqlOptions.</returns>
	public static NpgSqlOptions CreateDatabaseConfiguration(this PostgreSqlContainer container)
	{
		string connectionString = container.GetConnectionString();
		string[] pairs = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
		Dictionary<string, string> parameters = [];

		foreach (string pair in pairs)
		{
			string[] keyValuePair = pair.Split('=');
			string optionName = keyValuePair[0];
			string optionValue = keyValuePair[1];
			parameters.Add(optionName, optionValue);
		}

		string hostname = parameters["Host"];
		string port = parameters["Port"];
		string username = parameters["Username"];
		string password = parameters["Password"];
		string database = parameters["Database"];

		return new NpgSqlOptions()
		{
			Host = hostname,
			Port = port,
			Username = username,
			Password = password,
			Database = database,
		};
	}

	/// <summary>
	/// Создает конфигурацию RabbitMqOptions из контейнера RabbitMQ.
	/// </summary>
	/// <param name="container">Контейнер RabbitMQ.</param>
	/// <returns>Конфигурация RabbitMqOptions.</returns>
	public static RabbitMqOptions CreateRabbitMqConfiguration(this RabbitMqContainer container)
	{
		string connectionString = container.GetConnectionString();
		string[] parts = connectionString.Split('@', StringSplitOptions.RemoveEmptyEntries);
		string[] hostParts = parts[1].Split(':', StringSplitOptions.RemoveEmptyEntries);

		string host = hostParts[0];
		string port = hostParts[1].Replace("/", string.Empty, StringComparison.OrdinalIgnoreCase);
		string[] userParts = parts[0]
			.Split("//", StringSplitOptions.RemoveEmptyEntries)[1]
			.Split(':', StringSplitOptions.RemoveEmptyEntries);
		string username = userParts[0];
		string password = userParts[1];

		return new RabbitMqOptions()
		{
			Hostname = host,
			Port = int.Parse(port),
			Password = password,
			Username = username,
		};
	}
}
