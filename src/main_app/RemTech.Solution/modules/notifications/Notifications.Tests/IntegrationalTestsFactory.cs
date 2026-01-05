using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Notifications.Tests;

public sealed class IntegrationalTestsFactory : WebApplicationFactory<Notifications.WebApi.Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().BuildPgVectorContainer();
    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder().BuildRabbitMqContainer();
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rabbitMq.StartAsync();
        Services.ApplyModuleMigrations();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await _rabbitMq.StopAsync();
        await _rabbitMq.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(s =>
        {
            s.ReRegisterAppsettingsJsonConfiguration();
            s.ReRegisterRabbitMqOptions(_rabbitMq);
            s.ReRegisterNpgSqlOptions(_dbContainer);
            ReconfigureAesCryptography(s);
        });
    }

    private static void ReconfigureAesCryptography(IServiceCollection services)
    {
        services.RemoveAll<IConfigureOptions<AesEncryptionOptions>>();
        services.RemoveAll<IOptions<AesEncryptionOptions>>();
        services.AddOptions<AesEncryptionOptions>().BindConfiguration(nameof(AesEncryptionOptions));
    }
}