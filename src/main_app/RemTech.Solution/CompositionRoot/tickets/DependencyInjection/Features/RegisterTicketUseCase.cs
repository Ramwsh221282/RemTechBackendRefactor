using CompositionRoot.Shared;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Core.Contracts;
using Tickets.Logging;
using Tickets.Outbox.Features;
using Tickets.Persistence.UseCases;

namespace CompositionRoot.tickets.DependencyInjection.Features;

[DependencyInjectionClass]
internal static class RegisterTicketUseCase
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<RegisterTicket>(sp =>
        {
            RegisterTicket core = Tickets.Core.UseCases.RegisterTicketUseCase.RegisterTicket;
            RegisterTicket persisted = core.WithPersistence(sp);
            RegisterTicket outboxHandled = persisted.WithOutboxHandle(sp);
            RegisterTicket transaction = outboxHandled.WithTransaction(sp);
            RegisterTicket logging = transaction.WithLogging(sp);
            return logging;
        });
    }
}