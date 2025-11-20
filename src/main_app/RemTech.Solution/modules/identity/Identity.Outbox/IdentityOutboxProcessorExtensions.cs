using Identity.Outbox.Decorators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.NpgSql.Abstractions;
using RemTech.Outbox.Shared;
using RemTech.RabbitMq.Abstractions.Publishers;

namespace Identity.Outbox;

public static class IdentityOutboxProcessorExtensions
{
    extension(IServiceCollection services)
    {
        public void AddIdentityOutboxProcessor()
        {
            services.AddTransient<IIdentityOutboxProcessorWork>(sp =>
            {
                IHostApplicationLifetime lifetime = sp.GetRequiredService<IHostApplicationLifetime>();
                NpgSqlConnectionFactory connectionFactory = sp.Resolve<NpgSqlConnectionFactory>();
                OutboxServicesRegistry registry = sp.Resolve<OutboxServicesRegistry>();
                RabbitMqPublishers publishers = sp.Resolve<RabbitMqPublishers>();
                Serilog.ILogger logger = sp.Resolve<Serilog.ILogger>();
                
                CancellationTokenSource cts = new();
                lifetime.ApplicationStopping.Register(() => cts.Cancel());
                
                NpgSqlSession session = new(connectionFactory);
                OutboxService services = registry.GetService(session, "identity_module");

                IHowToProcessIdentityOutboxMessage howToOrigin = new HowToProcessIdentityOutboxMessage(publishers);
                IHowToProcessIdentityOutboxMessage howToLogging = new LoggingHowToProcessIdentityOutboxMessage(logger, howToOrigin);
                
                IdentityOutboxProcessorWork origin = new(services, howToLogging, cts.Token);
                TransactionalOutboxProcessorWork txn = new(logger, session, cts.Token, origin);
                LoggingTransactionalOutboxProcessorWork logging = new(logger, txn);
                return logging;
            });
            services.AddTransient<IdentityOutboxProcessor>();
            JobKey key = new(nameof(IdentityOutboxProcessor));
            services.AddQuartz(q =>
            {
                q.AddJob<IdentityOutboxProcessor>(c =>
                {
                    c.StoreDurably();
                    c.WithIdentity(key);
                }).AddTrigger(t =>
                {
                    t.ForJob(key).WithCronSchedule("*/5 * * * * ?");
                    t.WithIdentity($"trigger_{nameof(IdentityOutboxProcessor)}");
                });
            });
        }
    }
}