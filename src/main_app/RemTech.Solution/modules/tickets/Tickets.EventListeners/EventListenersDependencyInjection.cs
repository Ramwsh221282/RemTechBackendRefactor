using Microsoft.Extensions.DependencyInjection;
using Tickets.Core.Contracts;
using Tickets.EventListeners.Routers;
using Tickets.EventListeners.Routers.RequireActivationTicket;

namespace Tickets.EventListeners;

public static class TicketRoutersDependencyInjection
{
    public static void AddTicketRouters(this IServiceCollection services)
    {
        services.AddSingleton<ITicketRouter, RequireActivationTicketRouter>();
        services.AddSingleton<TicketRoutersRegistry>(sp =>
        {
            IEnumerable<ITicketRouter> routers = sp.GetServices<ITicketRouter>();
            TicketRoutersRegistry registry = new();
            
            foreach (var router in routers)
                registry.AddRouter(router);
            
            return registry;
        });
    }
}