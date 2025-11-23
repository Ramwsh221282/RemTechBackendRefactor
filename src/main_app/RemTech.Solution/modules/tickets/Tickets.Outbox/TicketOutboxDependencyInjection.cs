using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.NpgSql.Abstractions;
using RemTech.Outbox.Shared;
using Tickets.Core.Contracts;
using Tickets.Outbox.Decorators;

namespace Tickets.Outbox;

public static class TicketOutboxDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddTicketOutbox()
        {
            services.AddTransient<IDbUpgrader, TicketOutboxDbUpgrader>();
            services.AddTicketOutboxJob();
        }

        private void AddTicketOutboxJob()
        {
            services.AddTransient<ITicketsOutboxJobMethod>(sp =>
            {
                NpgSqlConnectionFactory npgSql = sp.Resolve<NpgSqlConnectionFactory>();
                NpgSqlSession session = new(npgSql);
                TicketRoutersRegistry routers = sp.Resolve<TicketRoutersRegistry>();
                OutboxServicesRegistry outboxes = sp.Resolve<OutboxServicesRegistry>();
                OutboxService outbox = outboxes.GetService(session, "tickets_module");
                Serilog.ILogger logger = sp.Resolve<Serilog.ILogger>();
                TicketsOutboxJobMethod core = new(routers, outbox, logger);
                TransactionalTicketsOutboxMessageMethod transactional = new(session, core);
                LoggingTicketsOutboxJobMethod logging = new(logger, transactional);
                return logging;
            });
            services.AddTransient<TicketsOutboxJob>();
            services.AddTransient<ICronScheduleJob, TicketsOutboxJob>();
        }
    }
}