using CompositionRoot.Shared;
using Mailing.Infrastructure.AesEncryption;
using Mailing.Infrastructure.InboxMessageProcessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RemTech.RabbitMq.Abstractions;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Tests.ModuleFixtures;

public sealed class CompositionRootFixture : WebApplicationFactory<WebHostApplication.Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().BuildPgVectorContainer();
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().BuildRabbitMqContainer();
    
    
    public IdentityModule IdentityModule => new(Services);
    public MailingModule MailingModule => new (Services);
    public TicketsModule TicketsModule => new(Services);

    public AsyncServiceScope Scope()
    {
        return Services.CreateAsyncScope();
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
        await _dbContainer.DisposeAsync();
        await _rabbitMqContainer.StopAsync();
        await _rabbitMqContainer.DisposeAsync();        
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(s =>
        {
            s.RegisterOutboxServices("identity_module", "tickets_module");
            s.RemoveAll<NpgSqlOptions>();
            s.RemoveAll<RabbitMqConnectionOptions>();
            ReconfigureInboxProcessorProcedure(s);
            ReconfigureAesEncryptionOptions(s);
            ReconfigureNpgSqlOptions(s);
            ReconfigureRabbitMqOptions(s);
        });
    }

    private void ReconfigureInboxProcessorProcedure(IServiceCollection services)
    {
        services.RemoveAll<InboxMessagesProcessorProcedure>();
        services.AddTransient<InboxMessagesProcessorProtocol, FakeInboxMessagesProcessorProcedure>();
    }
    
    private void ReconfigureAesEncryptionOptions(IServiceCollection services)
    {
        services.RemoveAll<IOptions<AesEncryptionOptions>>();
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        services.AddSingleton(configuration);
        services.AddOptions<AesEncryptionOptions>().BindConfiguration(nameof(AesEncryptionOptions));
    }
    
    private void ReconfigureRabbitMqOptions(IServiceCollection services)
    {
        services.RemoveAll<IOptions<RabbitMqConnectionOptions>>();
        IOptions<RabbitMqConnectionOptions> options = Options.Create(_rabbitMqContainer.CreateRabbitMqConfiguration());
        services.AddSingleton(options);
    }
    
    private void ReconfigureNpgSqlOptions(IServiceCollection services)
    {
        services.RemoveAll<IOptions<NpgSqlOptions>>();
        IOptions<NpgSqlOptions> options = Options.Create(_dbContainer.CreateDatabaseConfiguration());
        services.AddSingleton(options);
    }
}