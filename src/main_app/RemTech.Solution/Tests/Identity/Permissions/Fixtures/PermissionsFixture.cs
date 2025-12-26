using CompositionRoot.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;

namespace Tests.Identity.Permissions.Fixtures;

public sealed class PermissionsFixture :
    WebApplicationFactory<WebHostApplication.Program>,
    IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = 
        new PostgreSqlBuilder().BuildPgVectorContainer();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        Services.ApplyModuleMigrations();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(s =>
        {
            s.DontUseQuartzServices();
            s.ReconfigureConfigurationProvider();
            s.ReconfigureAesOptions();
            s.ReconfigurePostgreSqlOptions(_dbContainer);
        });
    }
}