using CompositionRoot.Shared;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Core.Contracts;
using Tickets.Logging;
using Tickets.Persistence.UseCases;

namespace CompositionRoot.tickets.DependencyInjection.Features;

[DependencyInjectionClass]
internal static class ActivateTicketUseCase
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<ActivateTicket>(sp =>
        {
            ActivateTicket core = Tickets.Core.UseCases.ActivateTicketUseCase.ActivateTicket;
            ActivateTicket peristed = core.WithPersistence(sp);
            ActivateTicket logging = peristed.WithLogging(sp);
            return logging;
        });
    }
}