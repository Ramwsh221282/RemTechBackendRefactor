using Identity.CompositionRoot;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RemTech.SharedKernel.Infrastructure.AesEncryption;
using RemTech.SharedKernel.Infrastructure.NpgSql;
using RemTech.SharedKernel.Infrastructure.Outbox;
using RemTech.SharedKernel.Infrastructure.Quartz;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace CompositionRoot.Shared;

public sealed class ClassNameLogEnricher : ILogEventEnricher
{
    private const string Pattern = "SourceContext";
    
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.TryGetValue(Pattern, out var sourceContext))
        {
            string fullName = sourceContext.ToString().Trim('\"');
            string exactTypeName = fullName.Split('.').Last();
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(Pattern, exactTypeName));
        }
    }
}

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public void RegisterSharedServices()
        {
            services.AddPostgres();
            services.AddAesCryptography();
            ILogger logger = new LoggerConfiguration()
                .Enrich.With(new ClassNameLogEnricher())
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message}{NewLine}{Exception}")
                .CreateLogger();
            services.AddSingleton(logger);
            services.AddRabbitMq();
        }
    
        public void RegisterModules()
        {
            services.AddIdentityModule();
        }
        
        public void RegisterOutboxServices(params string[] schemas)
        {
            services.AddOutboxServices(schemas);
        }
        
        public void AddQuartzJobs()
        {
            services.AddCronScheduledJobs();
            services.AddQuartzHostedService(c =>
            {
                c.AwaitApplicationStarted = true;
                c.WaitForJobsToComplete = true;
            });
        }
    }

    extension(IServiceProvider sp)
    {
        public void ApplyModuleMigrations()
        {
            PgVectorUpgrader mainUpgrader = sp.GetRequiredService<PgVectorUpgrader>();
            mainUpgrader.ApplyMigrations();
        
            IEnumerable<IDbUpgrader> upgraders = sp.GetServices<IDbUpgrader>();
            foreach (IDbUpgrader upgrader in upgraders)
                upgrader.ApplyMigrations();
        }
    }
}