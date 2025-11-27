using CompositionRoot.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;

namespace Tests.Identity.AccountPermissions.Fixture;

public sealed class AccountPermissionsFixture : WebApplicationFactory<WebHostApplication.Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder().BuildPgVectorContainer();
    
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        Services.ApplyModuleMigrations();
    }

    public async new Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(s =>
        {
            s.DontUseQuartzServices();
            s.ReconfigureConfigurationProvider();
            s.ReconfigureAesOptions();
            s.ReconfigurePostgreSqlOptions(_container);
        });
    }
}