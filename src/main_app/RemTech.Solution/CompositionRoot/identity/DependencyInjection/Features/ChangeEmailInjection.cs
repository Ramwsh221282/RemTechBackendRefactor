using CompositionRoot.Shared;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Logging;
using Identity.Persistence.NpgSql.SubjectsModule.Features;
using Microsoft.Extensions.DependencyInjection;

namespace CompositionRoot.identity.DependencyInjection.Features;

[DependencyInjectionClass]
internal static class ChangeEmailInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<ChangeEmail>(sp =>
        {
            ChangeEmail core = SubjectUseCases.ChangeEmail;
            ChangeEmail withPersistence = core.WithPersisting(sp);
            ChangeEmail withTransaction = withPersistence.WithTransaction(sp);
            ChangeEmail withLogging = withTransaction.WithLogging(sp);
            return withLogging;
        });
    }
}