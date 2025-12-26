using DromVehiclesParser.Parsing.ParsingStages;
using DromVehiclesParser.Shared.NpgSql;
using Quartz;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.Quartz;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace DromVehiclesParser.DependencyInjection;

public static class InfrastructureInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterInfrastructureDependencies(bool isDevelopment)
        {
            if (isDevelopment)
            {
                services.AddNpgSqlOptionsFromAppsettings();
                services.AddRabbitMqOptionsFromAppsettings();
            }
            services.AddMigrations([typeof(WorkStagesMigration).Assembly]);
            services.AddPostgres();
            services.AddRabbitMq();
            services.AddTransient<ICronScheduleJob, DummyCronScheduleJob>();
            services.AddSingleton<ICronScheduleJob, ParsingProcessInvoker>();
            services.AddCronScheduledJobs();
            services.AddQuartzHostedService(c =>
            {
                c.StartDelay = TimeSpan.FromSeconds(10);
                c.WaitForJobsToComplete = true;
            });
            
        }
    }
}