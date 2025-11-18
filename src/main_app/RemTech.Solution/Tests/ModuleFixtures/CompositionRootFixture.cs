using CompositionRoot.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Quartz;
using RemTech.RabbitMq.Abstractions;
using RemTech.Tests.Shared;
using Serilog;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Tests.Tickets;

namespace Tests.ModuleFixtures;

public sealed class CompositionRootFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().BuildPgVectorContainer();
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().BuildRabbitMqContainer();
    
    private readonly Lazy<IServiceProvider> _lazyProvider;
    private IServiceProvider Sp => _lazyProvider.Value;

    public IdentityModule IdentityModule { get; }
    public MailingModule MailingModule { get; }
    public TicketsModule TicketsModule { get; }

    public CompositionRootFixture()
    {
        _lazyProvider = new Lazy<IServiceProvider>(BuildProvider);
        IdentityModule = new(_lazyProvider);
        MailingModule = new(_lazyProvider);
        TicketsModule = new(_lazyProvider);
    }

    public AsyncServiceScope Scope()
    {
        return _lazyProvider.Value.CreateAsyncScope();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        Sp.ApplyModuleMigrations();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await _rabbitMqContainer.StopAsync();
        await _rabbitMqContainer.DisposeAsync();
    }

    private IServiceProvider BuildProvider()
    {
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        IServiceCollection services = new ServiceCollection();
        ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        services.AddSingleton(logger);
        services.AddPostgres();
        services.AddRabbitMq();
        services.RegisterModules();
        services.AddSingleton(configuration);
        services.AddQuartzHostedService(c => c.StartDelay = TimeSpan.FromSeconds(10));
        ReconfigureRabbitMqOptions(services);
        ReconfigureNpgSqlOptions(services);
        return services.BuildServiceProvider();
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