using Identity.Core.PermissionsModule;
using Identity.Core.PermissionsModule.Contracts;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Persistence.Features;
using Identity.UseCases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
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

        services.AddScoped<AddSubjectPermission>(sp =>
        {
            AddSubjectPermission origin = SubjectUseCases.AddSubjectPermission();
            AddSubjectPermission decorated = origin.WithPersisting(sp).WithTransaction(sp);
            return decorated;
        });

        services.AddScoped<ChangeEmail>(sp =>
        {
            ChangeEmail origin = SubjectUseCases.ChangeEmail;
            ChangeEmail decorated = origin.WithPersisting(sp);
            return decorated;
        });

        services.AddScoped<ChangePassword>(sp =>
        {
            ChangePassword origin = SubjectUseCases.ChangePassword;
            ChangePassword decorated = origin.WithPersisting(sp);
            return decorated;
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
    
    public async Task<Result<Permission>> RegisterPermission(string name)
    {
        await using AsyncServiceScope scope = Scope();
        CancellationToken ct = CancellationToken.None;
        RegisterPermissionArgs args = new(name, ct);
        RegisterPermission useCase = scope.Resolve<RegisterPermission>();
        return await useCase(args);
    }

    public async Task<Result<Subject>> ChangeSubjectEmail(Guid subjectId, string email)
    {
        ChangeEmailArgs args = new(subjectId, email, Optional<Subject>.None(), CancellationToken.None);
        await using AsyncServiceScope scope = Scope();
        ChangeEmail useCase = scope.Resolve<ChangeEmail>();
        return await useCase(args);
    }

    public async Task<Result<Subject>> ChangePassword(Guid subjectId, string password)
    {
        ChangePasswordArgs args = new(subjectId, password, Optional.None<Subject>(), CancellationToken.None);
        await using AsyncServiceScope scope = Scope();
        ChangePassword useCase = scope.Resolve<ChangePassword>();
        return await useCase(args);
    }
    
    public async Task<Result<Subject>> RegisterSubject(string login, string email, string password)
    {
        RegisterSubjectUseCaseArgs args = new(login, email, password, CancellationToken.None);
        await using AsyncServiceScope scope = Scope();
        RegisterSubject usecase = scope.Resolve<RegisterSubject>();
        return await usecase(args);
    }

    public async Task<Result<Subject>> UpdateSubject(Subject subject)
    {
        await using AsyncServiceScope scope = Scope();
        SubjectsStorage storage = scope.Resolve<SubjectsStorage>();
        await subject.UpdateIn(storage, CancellationToken.None);
        return subject;
    }
    
    public async Task<Result<Subject>> AddSubjectPermission(Guid subjectId, Guid permissionId)
    {
        AddSubjectPermissionArgs args = new(
            permissionId, 
            subjectId, 
            Optional.None<Subject>(), 
            Optional.None<Permission>(),
            CancellationToken.None);
        await using AsyncServiceScope scope = Scope();
        AddSubjectPermission useCase = scope.Resolve<AddSubjectPermission>();
        return await useCase(args);
    }
}