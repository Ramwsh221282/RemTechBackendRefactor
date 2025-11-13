using Identity.Core.SubjectsModule.Contracts;
using Identity.UseCases;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;

namespace Identity.PasswordHash.Tests;

public sealed class PasswordHashTestsFixture
{
    private readonly Lazy<IServiceProvider> _sp;

    public AsyncServiceScope Scope()
    {
        return _sp.Value.CreateAsyncScope();
    }
    
    public PasswordHashTestsFixture()
    {
        _sp = new Lazy<IServiceProvider>(BuildServiceProvider);
    }

    private IServiceProvider BuildServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddIdentityPasswordHashing();
        services.AddScoped<RegisterSubject>(sp =>
        {
            RegisterSubject origin = RegisterSubjectUseCase.RegisterSubject();
            HashPassword hash = sp.Resolve<HashPassword>();
            return origin.WithPasswordHashing(hash);
        });

        return services.BuildServiceProvider();
    }
}