using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Tests.ParserStartTests;
using Tests.ParserSubscriptionTests;

namespace Tests;

public sealed class IntegrationalTestsFixture : WebApplicationFactory<DromVehiclesParser.Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().BuildPgVectorContainer();
    private readonly RabbitMqContainer _brokerContainer = new RabbitMqBuilder().BuildRabbitMqContainer();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(s =>
        {
            s.ReRegisterCronScheduleJobs();
            s.ReRegisterQuartzHostedService(c =>
            {
                c.WaitForJobsToComplete = true;
                c.WaitForJobsToComplete = true;
            });
            s.ReRegisterAppsettingsJsonConfiguration();
            s.ReRegisterRabbitMqOptions(_brokerContainer);
            s.ReRegisterNpgSqlOptions(_dbContainer);
            s.AddHostedService<FakeParserSubscriptionQueue>();
            s.AddTransient<FakeParserSubscriptionPublisher>();
            s.AddTransient<FakeParserStartPublisher>();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _brokerContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _brokerContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await _brokerContainer.DisposeAsync();
    }
}