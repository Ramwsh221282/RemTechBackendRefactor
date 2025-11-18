using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using Serilog;
using Tickets.Core;
using Tickets.Core.Contracts;

namespace Tickets.Logging;

public static class ActivateTicketUseCase
{
    public static ActivateTicket ActivateTicket(ActivateTicket origin, ILogger logger) => async args =>
    {
        Result<Ticket> result = await origin(args);
        object[] parameters = [args.CreatorId, result.IsSuccess, result.Error.Message];
        
        if (result.IsFailure)
        {
            logger.Error("""
                         Активация заявки
                         Creator Id {Id}
                         IsSuccess {IsSuccess}
                         Error {Error}
                         """, parameters);
        }
        
        if (result.IsSuccess)
        {
            logger.Error("""
                         Активация заявки
                         Creator Id {Id}
                         IsSuccess {IsSuccess}
                         """, parameters);
        }

        return result;
    };

    extension(ActivateTicket origin)
    {
        public ActivateTicket WithLogging(IServiceProvider sp)
        {
            return ActivateTicket(origin, sp.Resolve<ILogger>());
        }
    }
}