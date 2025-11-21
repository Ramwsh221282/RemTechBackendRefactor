using Tickets.Core.Contracts;

namespace Tickets.EventListeners.Routers;

public sealed class TicketRoutersRegistry
{
    private readonly Dictionary<string, ITicketRouter> _routers = [];

    public bool TryGetRouter(in string type, out ITicketRouter? router)
    {
        return  _routers.TryGetValue(type, out router);
    }
    
    public void AddRouter(in ITicketRouter router)
    {
        if (_routers.ContainsKey(router.SupportedTicketType))
            return;
        
        TicketRouterEnvelope envelope = new TypeValidatedTicketRouter(router);
        _routers.Add(router.SupportedTicketType, envelope);
    }
}