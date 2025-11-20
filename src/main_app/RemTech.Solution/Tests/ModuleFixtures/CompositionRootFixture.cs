using CompositionRoot.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Quartz;
using RemTech.RabbitMq.Abstractions;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Tickets.EventListeners;

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
            s.RemoveAll<NpgSqlOptions>();
            s.RemoveAll<RabbitMqConnectionOptions>();
            s.RemoveAll<TicketCreatedEventListener>();
            ReconfigureNpgSqlOptions(s);
            ReconfigureRabbitMqOptions(s);
            s.AddQuartzHostedService(c =>
            {
                c.AwaitApplicationStarted = true;
                c.WaitForJobsToComplete = true;
            });
        });
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