using CompositionRoot.Shared;
using Identity.Persistence.NpgSql;
using Identity.Persistence.NpgSql.PermissionsModule;
using Identity.Persistence.NpgSql.SubjectsModule;
using Identity.Persistence.NpgSql.SubjectsModule.SubjectTickets;
using Microsoft.Extensions.DependencyInjection;
using RemTech.NpgSql.Abstractions;

namespace CompositionRoot.identity.DependencyInjection.IdentityPersisting;

[DependencyInjectionClass]
internal static class IdentityPersistenceInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddTransient<IDbUpgrader, IdentityDbUpgrader>();
        services.AddSubjectsStorage();
        services.AddSubjectTicketsPersistence();
        services.AddPermissionsStorage();
    }
}