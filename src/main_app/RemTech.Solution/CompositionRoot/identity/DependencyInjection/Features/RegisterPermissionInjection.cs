using CompositionRoot.Shared;
using Identity.Core.PermissionsModule.Contracts;
using Identity.Logging;
using Identity.Persistence.NpgSql.SubjectsModule.Features;
using Microsoft.Extensions.DependencyInjection;

namespace CompositionRoot.identity.DependencyInjection.Features;

[DependencyInjectionClass]
public static class RegisterPermissionInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<RegisterPermission>(sp =>
        {
            RegisterPermission origin = PermissionUseCases.DefaultRegisterPermission;
            RegisterPermission withPersistence = origin.WithPersisting(sp);
            RegisterPermission withLogging = withPersistence.WithLogging(sp);
            return withLogging;
        });
    }
}