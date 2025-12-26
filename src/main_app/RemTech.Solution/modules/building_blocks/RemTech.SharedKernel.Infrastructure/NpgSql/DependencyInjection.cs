using System.Reflection;
using Dapper;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Pgvector.Dapper;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.NpgSql.Migrations;

namespace RemTech.SharedKernel.Infrastructure.NpgSql;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddPostgres()
        {
            services.AddOptions<NpgSqlOptions>().BindConfiguration(nameof(NpgSqlOptions));
            services.TryAddSingleton<NpgSqlConnectionFactory>();
            services.TryAddScoped<NpgSqlSession>();
            services.TryAddScoped<ITransactionSource, NpgSqlTransactionSource>();
            SqlMapper.AddTypeHandler(new VectorTypeHandler());
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public void AddMigrations(Assembly[] assemblies)
        {
            Assembly[] withPgVectorAssembly = [typeof(PgVectorMigration).Assembly, ..assemblies];
            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(ConnectionString)
                    .ScanIn(withPgVectorAssembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());
        }
    }

    private static string ConnectionString(IServiceProvider services)
    {
        return services.GetRequiredService<IOptions<NpgSqlOptions>>().Value.ToConnectionString();
    }    
    extension(IServiceProvider provider)
    {
        public void ApplyModuleMigrations()
        {
            using IServiceScope scope = provider.CreateScope();
            IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            SortedList<long, IMigrationInfo> migrations = runner.MigrationLoader.LoadMigrations();
            foreach (KeyValuePair<long, IMigrationInfo> migrationPair in migrations)
            {
                IMigrationInfo migration = migrationPair.Value;
                runner.Up(migration.Migration);
            }
        }

        public void RollBackModuleMigrations()
        {
            IServiceScope scope = provider.CreateScope();
            IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            Dictionary<long, IMigrationInfo> migrations = runner
                .MigrationLoader.LoadMigrations()
                .OrderByDescending(m => m.Key)
                .ToDictionary(m => m.Key, m => m.Value);
            
            foreach (var migration in migrations)
                runner.Down(migration.Value.Migration);
        }
    }
}