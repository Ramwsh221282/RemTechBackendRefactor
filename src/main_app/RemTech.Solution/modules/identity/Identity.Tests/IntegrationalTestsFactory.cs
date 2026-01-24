using Identity.Infrastructure.Common;
using Identity.Tests.Fakes;
using Identity.WebApi.BackgroundServices;
using Identity.WebApi.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTech.SharedKernel.Infrastructure.Redis;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Identity.Tests;

public sealed class IntegrationalTestsFactory : WebApplicationFactory<Identity.WebApi.Program>, IAsyncLifetime
{
	private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().BuildPgVectorContainer();
	private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().BuildRabbitMqContainer();
	private readonly RedisContainer _redisContainer = new RedisBuilder().BuildRedisContainer();

	public async Task InitializeAsync()
	{
		await _dbContainer.StartAsync();
		await _rabbitMqContainer.StartAsync();
		await _redisContainer.StartAsync();
		Services.ApplyModuleMigrations();
	}

	public new async Task DisposeAsync()
	{
		await _dbContainer.StopAsync();
		await _dbContainer.DisposeAsync();
		await _rabbitMqContainer.StopAsync();
		await _rabbitMqContainer.DisposeAsync();
		await _redisContainer.StopAsync();
		await _redisContainer.DisposeAsync();
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		base.ConfigureWebHost(builder);
		builder.ConfigureServices(s =>
		{
			s.ReRegisterAppsettingsJsonConfiguration();
			ReRegisterSuperUserOptionsSettings(s);
			ReRegisterWorkFactorOptionsSettings(s);
			ReRegisterCachingOptionsSettings(s);
			ReRegisterJwtOptionsSettings(s);
			s.ReRegisterNpgSqlOptions(_dbContainer);
			s.ReRegisterRabbitMqOptions(_rabbitMqContainer);
			s.ReRegisterBackgroundService<SuperUserAccountPermissionsUpdateBackgroundServices>();
			s.ReRegisterBackgroundService<SuperUserAccountRegistrationOnStartupBackgroundService>();
			s.ReRegisterBackgroundService<AccountsModuleOutboxProcessor>();
			s.AddSingleton<IConsumer, FakeOnUserAccountRegisteredConsumer>();
			s.AddHostedService<AggregatedConsumersHostedService>();
		});
	}

	private void ReRegisterCachingOptionsSettings(IServiceCollection services)
	{
		services.RemoveAll<IConfigureOptions<CachingOptions>>();
		services.RemoveAll<IOptions<CachingOptions>>();

		IOptions<CachingOptions> options = Options.Create(
			new CachingOptions()
			{
				RedisConnectionString = _redisContainer.GetConnectionString(),
				CacheExpirationMinutes = 10,
				LocalCacheExpirationMinutes = 5,
			}
		);

		services.AddSingleton(options);
	}

	private void ReRegisterJwtOptionsSettings(IServiceCollection services)
	{
		services.RemoveAll<IConfigureOptions<JwtOptions>>();
		services.RemoveAll<IOptions<JwtOptions>>();
		services.AddOptions<JwtOptions>().BindConfiguration(nameof(JwtOptions));
	}

	private void ReRegisterSuperUserOptionsSettings(IServiceCollection services)
	{
		services.RemoveAll<IConfigureOptions<SuperUserCredentialsOptions>>();
		services.RemoveAll<IOptions<SuperUserCredentialsOptions>>();
		services.AddOptions<SuperUserCredentialsOptions>().BindConfiguration(nameof(SuperUserCredentialsOptions));
	}

	private void ReRegisterWorkFactorOptionsSettings(IServiceCollection services)
	{
		services.RemoveAll<IConfigureOptions<BcryptWorkFactorOptions>>();
		services.RemoveAll<IOptions<BcryptWorkFactorOptions>>();
		services.AddOptions<BcryptWorkFactorOptions>().BindConfiguration(nameof(BcryptWorkFactorOptions));
	}
}
