using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.Tests.Shared;
using RemTechAvitoVehiclesParser;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Tests.ParserSubscriptionTests;

public sealed class ParserSubscriptionFixture : WebApplicationFactory<RemTechAvitoVehiclesParser.Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().BuildPgVectorContainer();
    private readonly RabbitMqContainer _brokerContainer = new RabbitMqBuilder().BuildRabbitMqContainer();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(s =>
        {
            s.ReRegisterAppsettingsJsonConfiguration();
            s.ReRegisterNpgSqlOptions(_dbContainer);
            s.ReRegisterRabbitMqOptions(_brokerContainer);
            s.AddHostedService<FakeParserSubscriptionQueue>();
            s.AddTransient<FakeParserSubscriptionQueuePublisher>();
            s.RegisterParserSubscription();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _brokerContainer.StartAsync();
        Services.ApplyModuleMigrations();
    }

    public async Task DisposeAsync()
    {
        await _brokerContainer.StopAsync();
        await _dbContainer.StopAsync();
        await _brokerContainer.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }
}