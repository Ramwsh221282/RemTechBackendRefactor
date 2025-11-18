using CompositionRoot.Shared;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Logging;
using Identity.Persistence.NpgSql.SubjectsModule.Features;
using Microsoft.Extensions.DependencyInjection;

namespace CompositionRoot.identity.DependencyInjection.Features;

[DependencyInjectionClass]
internal static class ChangePasswordInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<ChangePassword>(sp =>
        {
            ChangePassword core = SubjectUseCases.ChangePassword;
            ChangePassword withPersistence = core.WithPersisting(sp);
            ChangePassword withTranscation = withPersistence.WithTranscation(sp);
            ChangePassword withLogging = withTranscation.WithLogging(sp);
            return withLogging;
        });
    }
}