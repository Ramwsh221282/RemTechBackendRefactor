using CompositionRoot.Shared;
using RemTech.Tests.Shared;
using Serilog;
using Testcontainers.PostgreSql;

namespace Tests.ModuleFixtures;

public sealed class CompositionRootFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder().BuildPgVectorContainer();
    private readonly Lazy<IServiceProvider> _lazyProvider;
    private IServiceProvider Sp => _lazyProvider.Value;

    public IdentityModule IdentityModule { get; }
    public MailingModule MailingModule { get; }

    public CompositionRootFixture()
    {
        _lazyProvider = new Lazy<IServiceProvider>(BuildProvider);
        IdentityModule = new(Sp);
        MailingModule = new(Sp);
    }

    public AsyncServiceScope Scope()
    {
        return _lazyProvider.Value.CreateAsyncScope();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        Sp.ApplyModuleMigrations();
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    private IServiceProvider BuildProvider()
    {
        IServiceCollection services = new ServiceCollection();
        ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        services.AddSingleton(logger);
        services.AddPostgres();
        services.RegisterModules();
        return services.BuildServiceProvider();
    }
}