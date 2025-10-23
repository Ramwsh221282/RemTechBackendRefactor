using System.Data;
using Identity.Adapter.Storage;
using Identity.Domain.Users.Aggregate;
using Identity.Notifier.Port;
using Identity.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using RemTech.Shared.Tests;
using Shared.Infrastructure.Module.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Identity.Integrational.Tests;

public sealed class IdentityTestApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder().BuildPgVectorContainer();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        DatabaseOptions dbOptions = _dbContainer.CreateDatabaseConfiguration();

        builder.ConfigureTestServices(sp =>
        {
            sp.RemoveAll<IOptions<DatabaseOptions>>();
            sp.RemoveAll<IdentityDbContext>();

            IOptions<DatabaseOptions> options = Options.Create(dbOptions);
            sp.AddSingleton(options);
            sp.AddScoped<IdentityDbContext>();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        await using IdentityDbContext context = scope.GetService<IdentityDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
}
