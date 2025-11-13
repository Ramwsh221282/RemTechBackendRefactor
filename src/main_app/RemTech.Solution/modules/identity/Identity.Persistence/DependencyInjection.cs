using System.Reflection;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.TicketsModule.Contracts;
using Identity.Persistence.NpgSql;
using Identity.Persistence.NpgSql.TicketsModule;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence;

public static class DependencyInjection
{
    private static readonly Assembly Assembly = typeof(NpgSqlIdentitySubjects).Assembly;
    
    extension(IServiceCollection services)
    {
        public void AddIdentityPersistenceModule()
        {
            services.AddPostgres();
            services.AddKeyedSingleton<IDbUpgrader, IdentityDbUpgrader>(nameof(Identity));
            services.AddSubjectPersistenceDependencies();
            services.AddTicketsPersistenceDependencies();
        }
        
        private void AddSubjectPersistenceDependencies()
        {
            services.AddScopedDelegate<InsertSubject>(Assembly);
            services.AddScopedDelegate<DeleteSubject>(Assembly);
            services.AddScopedDelegate<UpdateSubject>(Assembly);
            services.AddScopedDelegate<IsSubjectEmailUnique>(Assembly);
            services.AddScopedDelegate<IsSubjectLoginUnique>(Assembly);
            services.AddScoped<NpgSqlIdentitySubjectCommands>();
        }
        
        private void AddTicketsPersistenceDependencies()
        {
            services.AddScopedDelegate<InsertTicket>(Assembly);
            services.AddScopedDelegate<DeleteTicket>(Assembly);
            services.AddScopedDelegate<UpdateTicket>(Assembly);
            services.AddScopedDelegate<GetTicket>(Assembly);
            services.AddScoped<NpgSqlTicketsCommands>();
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