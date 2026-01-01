using System.Reflection;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Pgvector.Dapper;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTech.SharedKernel.Infrastructure.Database;

public static class DatabaseExtensions
{
    extension(IServiceCollection services)
    {
        public void AddPostgres()
        {
            if (!services.ContainsOptions())
                throw new InvalidOperationException($"IOptions of {nameof(NpgSqlOptions)} not configured. Please configure it before calling AddPostgres.");
            services.TryAddSingleton<NpgSqlConnectionFactory>();
            services.TryAddScoped<NpgSqlSession>();
            services.TryAddScoped<ITransactionSource, NpgSqlTransactionSource>();
            SqlMapper.AddTypeHandler(new VectorTypeHandler());
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public void AddMigrations(Assembly[] assemblies)
        {
            Assembly[] withPgVectorAssembly = [..assemblies, typeof(PgVectorMigration).Assembly];
            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(
                        sp => sp.GetRequiredService<IOptions<NpgSqlOptions>>().Value.ToConnectionString())
                    .ScanIn(withPgVectorAssembly).For.All())
                .AddLogging(lb => lb.AddFluentMigratorConsole());
        }

        private bool ContainsOptions()
        {
            return services.Any(s => s.ServiceType == typeof(IOptions<NpgSqlOptions>) ||
                                     s.ServiceType == typeof(IConfigureOptions<NpgSqlOptions>));
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
            runner.MigrateUp();
        }

        public void RollBackModuleMigrations()
        {
            using IServiceScope scope = provider.CreateScope();
            IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            var appliedCount = runner.MigrationLoader.LoadMigrations()
                .Count(m => m.Key > PgVectorMigration.Version);

            if (appliedCount > 0)
            {
                runner.Rollback(appliedCount);
            }
        }
    }
}