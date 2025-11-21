using Identity.Outbox.Decorators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.NpgSql.Abstractions;
using RemTech.Outbox.Shared;
using RemTech.RabbitMq.Abstractions.Publishers;

namespace Identity.Outbox;

public static class IdentityOutboxInjection
{
    public static void AddIdentityOutbox(this IServiceCollection services)
    {
        services.AddTransient<IDbUpgrader, IdentityOutboxDbUpgrader>();
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
            OutboxService outbox = registry.GetService(session, "identity_module");
        
            IHowToProcessIdentityOutboxMessage howToOrigin = new HowToProcessIdentityOutboxMessage(publishers);
            IHowToProcessIdentityOutboxMessage howToLogging = new LoggingHowToProcessIdentityOutboxMessage(logger, howToOrigin);
                
            IdentityOutboxProcessorWork origin = new(outbox, howToLogging, cts.Token);
            TransactionalOutboxProcessorWork txn = new(logger, session, cts.Token, origin);
            LoggingTransactionalOutboxProcessorWork logging = new(logger, txn);
            return logging;
        });
        services.AddTransient<IdentityOutboxProcessor>();
        services.AddTransient<ICronScheduleJob, IdentityOutboxProcessor>();
    }
}