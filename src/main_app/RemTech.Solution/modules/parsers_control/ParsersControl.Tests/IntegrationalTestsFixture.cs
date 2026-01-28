using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Contracts;
using ParsersControl.Tests.Parsers.SubscribeParser;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace ParsersControl.Tests;

/// <summary>
/// Фикстура для интеграционных тестов модуля ParsersControl.
/// </summary>
public sealed class IntegrationalTestsFixture : WebApplicationFactory<WebApi.Program>, IAsyncLifetime
{
	private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().BuildPgVectorContainer();
	private readonly RabbitMqContainer _rabbitContainer = new RabbitMqBuilder().BuildRabbitMqContainer();

	/// <summary>
	/// Инициализация фикстуры.
	/// </summary>
	/// <returns>Задача.</returns>
	public async Task InitializeAsync()
	{
		await _dbContainer.StartAsync();
		await _rabbitContainer.StartAsync();
		Services.ApplyModuleMigrations();
	}

	/// <summary>
	/// Освобождение ресурсов фикстуры.
	/// </summary>
	/// <returns>Задача.</returns>
	public new async Task DisposeAsync()
	{
		await _dbContainer.StopAsync();
		await _rabbitContainer.StopAsync();
	}

	/// <summary>
	/// Конфигурация веб-хоста для тестов.
	/// </summary>
	/// <param name="builder">Построитель веб-хоста.</param>
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		base.ConfigureWebHost(builder);

		builder.ConfigureServices(s =>
		{
			s.AddPostgres();
			s.RegisterLogging();
			s.ReRegisterNpgSqlOptions(_dbContainer);
			s.ReRegisterRabbitMqOptions(_rabbitContainer);
			s.AddScoped<IOnParserSubscribedListener, FakeOnParserSubscribedListener>();
			s.AddScoped<IOnParserStartedListener, FakeOnParserWorkStartedListener>();
			s.AddTransient<FakeParserSubscribeProducer>();
		});
	}
}
