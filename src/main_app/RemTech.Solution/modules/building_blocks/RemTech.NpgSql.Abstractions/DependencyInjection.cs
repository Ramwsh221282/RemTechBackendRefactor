using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Pgvector.Dapper;

namespace RemTech.NpgSql.Abstractions;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddPostgres()
        {
            services.AddSingleton<NpgSqlConnectionFactory>();
            SqlMapper.AddTypeHandler(new VectorTypeHandler());
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            services.AddKeyedSingleton<IDbUpgrader, PgVectorUpgrader>(nameof(PgVectorUpgrader));
        }
    }
}