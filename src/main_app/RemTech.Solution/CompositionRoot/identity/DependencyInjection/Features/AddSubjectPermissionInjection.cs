using CompositionRoot.Shared;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Logging;
using Identity.Persistence.NpgSql.SubjectsModule.Features;
using Microsoft.Extensions.DependencyInjection;

namespace CompositionRoot.identity.DependencyInjection.Features;

[DependencyInjectionClass]
internal static class AddSubjectPermissionInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<AddSubjectPermission>(sp =>
        {
            AddSubjectPermission core = SubjectUseCases.AddSubjectPermission;
            AddSubjectPermission withPersistence = core.WithPersisting(sp);
            AddSubjectPermission withTransaction = withPersistence.WithTransaction(sp);
            AddSubjectPermission withLogging = withTransaction.WithLogging(sp);
            return withLogging;
        });
    }
}