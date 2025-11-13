using Identity.Core.PermissionsModule.Contracts;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Persistence.Features;
using Identity.UseCases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.NpgSql.Abstractions;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;

namespace Identity.Persistence.Tests;

public sealed class IdentityPersistenceTestsFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder().BuildPgVectorContainer();
    
    private readonly Lazy<IServiceProvider> _sp;

    public AsyncServiceScope Scope()
    {
        return _sp.Value.CreateAsyncScope();
    }

    public IdentityPersistenceTestsFixture()
    {
        _sp = new Lazy<IServiceProvider>(BuildServiceProvider);
    }

    private IServiceProvider BuildServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        services.AddSingleton(configuration);
        
        services.AddSingleton(Options.Create(_container.CreateDatabaseConfiguration()));
        services.AddPostgres();
        services.AddIdentityPersistenceModule();
        
        services.AddScoped<RegisterSubject>(sp =>
        {
            RegisterSubject origin = RegisterSubjectUseCase.RegisterSubject();
            RegisterSubject withPersistence = origin.WithPersistence(sp.Resolve<SubjectsStorage>());
            return withPersistence;
        });

        services.AddScoped<RegisterPermission>(sp =>
        {
            RegisterPermission origin = PermissionUseCases.DefaultRegisterPermission;
            RegisterPermission withPersistence = origin.WithPersisting(sp);
            return withPersistence;
        });
        
        return services.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        _sp.Value.ApplyPgVectorMigrations();
        _sp.Value.ApplyIdentityPersistenceMigrations();
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}