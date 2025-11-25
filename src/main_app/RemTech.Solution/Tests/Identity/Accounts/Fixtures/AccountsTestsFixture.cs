using CompositionRoot.Shared;
using Identity.Contracts.Accounts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;

namespace Tests.Identity.Accounts.Fixtures;

public sealed class AccountsTestsFixture : 
    WebApplicationFactory<WebHostApplication.Program>,
    IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().BuildPgVectorContainer();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        Services.ApplyModuleMigrations();
    }

    public async new Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(s =>
        {
            s.ReconfigurePostgreSqlOptions(_dbContainer);
            s.ReconfigureConfigurationProvider();
            s.ReconfigureAesOptions();
            s.AddScoped<IAccountMessagePublisher, FakeAccountMessagePublisher>();
        });
    }
}