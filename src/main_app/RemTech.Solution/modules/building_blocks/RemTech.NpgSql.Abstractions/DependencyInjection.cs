using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pgvector.Dapper;

namespace RemTech.NpgSql.Abstractions;

public static class DependencyInjection
{
    private static bool _hasConfigured = false;
    
    extension(IServiceCollection services)
    {
        public void AddPostgres()
        {
            if (_hasConfigured) return;
            services.TryAddSingleton<NpgSqlConnectionFactory>();
            services.AddTransient<IDbUpgrader, PgVectorUpgrader>();
            services.TryAddScoped<NpgSqlSession>();
            SqlMapper.AddTypeHandler(new VectorTypeHandler());
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            _hasConfigured = true;
        }
    }

    extension(IServiceProvider provider)
    {
        public void ApplyMigrationsFor(string dbUpgraderName)
        {
            provider.GetRequiredKeyedService<IDbUpgrader>(dbUpgraderName).ApplyMigrations();
        }

        public void ApplyPgVectorMigrations()
        {
            provider.GetRequiredKeyedService<IDbUpgrader>(nameof(PgVectorUpgrader)).ApplyMigrations();
        }
    }
}