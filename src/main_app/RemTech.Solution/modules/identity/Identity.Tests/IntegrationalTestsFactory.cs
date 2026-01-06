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
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Identity.Tests;

public sealed class IntegrationalTestsFactory : WebApplicationFactory<Identity.WebApi.Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().BuildPgVectorContainer();
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().BuildRabbitMqContainer();
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        Services.ApplyModuleMigrations();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await _rabbitMqContainer.StopAsync();
        await _rabbitMqContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(s =>
        {
            s.ReRegisterAppsettingsJsonConfiguration();
            ReRegisterSuperUserOptionsSettings(s);
            ReRegisterWorkFactorOptionsSettings(s);
            s.ReRegisterNpgSqlOptions(_dbContainer);
            s.ReRegisterRabbitMqOptions(_rabbitMqContainer);
            s.ReRegisterBackgroundService<SuperUserAccountPermissionsUpdateBackgroundServices>();
            s.ReRegisterBackgroundService<SuperUserAccountRegistrationOnStartupBackgroundService>();
            s.ReRegisterBackgroundService<AccountsModuleOutboxProcessor>();
            s.AddSingleton<IConsumer, FakeOnUserAccountRegisteredConsumer>();
            s.AddHostedService<AggregatedConsumersHostedService>();
        });
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