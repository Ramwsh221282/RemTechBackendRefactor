using CompositionRoot.Shared;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Persistence.NpgSql.SubjectsModule.Features;
using Microsoft.Extensions.DependencyInjection;

namespace CompositionRoot.identity.DependencyInjection.Features;

[DependencyInjectionClass]
internal static class RegisterSubjectInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<RegisterSubject>(sp =>
        {
            RegisterSubject core = SubjectUseCases.RegisterSubject;
            RegisterSubject withHash = core.WithPasswordHashing(sp);
            RegisterSubject withPersistence = withHash.WithPersistence(sp);
            RegisterSubject withLogging = withPersistence.WithLogging(sp);
            return withLogging;
        });
    }
}