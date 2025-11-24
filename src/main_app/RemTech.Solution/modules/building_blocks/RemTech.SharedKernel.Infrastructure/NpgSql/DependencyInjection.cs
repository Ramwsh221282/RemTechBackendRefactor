using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pgvector.Dapper;

namespace RemTech.SharedKernel.Infrastructure.NpgSql;

public static class DependencyInjection
{
    private static bool _hasConfigured;
    
    extension(IServiceCollection services)
    {
        public void AddPostgres()
        {
            if (_hasConfigured) return;
            services.AddOptions<NpgSqlOptions>().BindConfiguration(nameof(NpgSqlOptions));
            services.AddTransient<PgVectorUpgrader>();
            services.TryAddSingleton<NpgSqlConnectionFactory>();
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