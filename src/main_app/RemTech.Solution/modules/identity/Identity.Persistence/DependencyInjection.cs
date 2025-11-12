using System.Reflection;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Persistence.NpgSql;
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
            services.AddScopedDelegate<InsertSubject>(Assembly);
            services.AddScopedDelegate<DeleteSubject>(Assembly);
            services.AddScopedDelegate<UpdateSubject>(Assembly);
            services.AddScopedDelegate<IsSubjectEmailUnique>(Assembly);
            services.AddScopedDelegate<IsSubjectLoginUnique>(Assembly);
            services.AddScoped<NpgSqlIdentitySubjectCommands>();
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