using CompositionRoot.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Tests.Identity.Accounts.Fixtures;

public sealed class AccountsTestsFixture : 
    WebApplicationFactory<WebHostApplication.Program>,
    IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().BuildPgVectorContainer();
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().BuildRabbitMqContainer();
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        Services.ApplyModuleMigrations();
    }

    public async new Task DisposeAsync()
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
            s.ReconfigureConfigurationProvider();
            s.ReconfigureAesOptions();
            s.ReconfigureRabbitMqOptions(_rabbitMqContainer);
            s.ReconfigurePostgreSqlOptions(_dbContainer);
            s.ReconfigureQuartzHostedService();
        });
    }
}