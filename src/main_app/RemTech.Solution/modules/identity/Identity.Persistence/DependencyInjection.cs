using Identity.Persistence.NpgSql;
using Identity.Persistence.NpgSql.PermissionsModule;
using Identity.Persistence.NpgSql.SubjectsModule;
using Identity.Persistence.NpgSql.SubjectsModule.SubjectTickets;
using Identity.Persistence.NpgSql.TicketsModule;
using Microsoft.Extensions.DependencyInjection;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddIdentityPersistenceModule()
        {
            services.AddPostgres();
            services.AddKeyedSingleton<IDbUpgrader, IdentityDbUpgrader>(nameof(Identity));
            services.AddSubjectsStorage();
            services.AddSubjectTicketsPersistence();
            services.AddTicketsPersistence();
            services.AddPermissionsStorage();
        }
    }

    extension(IServiceProvider provider)
    {
        public void ApplyIdentityPersistenceMigrations()
        {
            provider.ApplyPgVectorMigrations();
            provider.ApplyMigrationsFor(nameof(Identity));
        }
    }
}