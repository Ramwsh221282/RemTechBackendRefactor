using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.DependencyInjection;
using RemTech.Infrastructure.PostgreSQL;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;
using Vehicles.Infrastructure.PostgreSQL;
using Vehicles.WebApi;

namespace Vehicles.Tests.Configurations;

public sealed class VehiclesApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder().FormPgVectorContainer();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<NpgsqlOptions>();
            services.RemoveAll<VehiclesServiceDbContext>();

            NpgsqlOptions options = _dbContainer.CreateNpgsqlOptions();
            services.AddSingleton(options);
            services.AddScoped<VehiclesServiceDbContext>();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        await using VehiclesServiceDbContext context = scope.GetService<VehiclesServiceDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
