using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pgvector.Dapper;
using RemTech.SharedKernel.Core.InfrastructureContracts;

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
            services.TryAddScoped<ITransactionSource, NpgSqlTransactionSource>();
            SqlMapper.AddTypeHandler(new VectorTypeHandler());
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            _hasConfigured = true;
        }
    }

    extension(IServiceProvider provider)
    {
        public void ApplyModuleMigrations()
        {
            PgVectorUpgrader mainUpgrader = provider.GetRequiredService<PgVectorUpgrader>();
            mainUpgrader.ApplyMigrations();
        
            IEnumerable<IDbUpgrader> upgraders = provider.GetServices<IDbUpgrader>();
            foreach (IDbUpgrader upgrader in upgraders)
                upgrader.ApplyMigrations();
        }
    }
}