using CompositionRoot.Shared;
using Mailing.Infrastructure.InboxMessageProcessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Tests.IdentityMailingCommunication;

public sealed class IdentityMailingCommunicationFixture : WebApplicationFactory<WebHostApplication.Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().BuildPgVectorContainer();
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().BuildRabbitMqContainer();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(s =>
        {
            s.ReconfigureConfigurationProvider();
            s.ReconfigureAesOptions();
            s.ReconfigurePostgreSqlOptions(_dbContainer);
            s.ReconfigureRabbitMqOptions(_rabbitMqContainer);
            s.ReconfigureQuartzHostedService();
            s.RemoveAll<InboxMessagesProcessorProtocol>();
            s.TryAddTransient<InboxMessagesProcessorProtocol, FakeInboxMessagesProcessorProcedure>();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        Services.ApplyModuleMigrations();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _rabbitMqContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await _rabbitMqContainer.DisposeAsync();
    }
}