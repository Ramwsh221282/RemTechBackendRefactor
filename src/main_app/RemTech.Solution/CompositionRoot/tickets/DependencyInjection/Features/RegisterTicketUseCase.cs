using CompositionRoot.Shared;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Core.Contracts;
using Tickets.Logging;
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
            RegisterTicket logging = core.WithLogging(sp);
            RegisterTicket persisted = logging.WithPersistence(sp);
            return persisted;
        });
    }
}