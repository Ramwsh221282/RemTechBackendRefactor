using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using Serilog;
using Tickets.Core;
using Tickets.Core.Contracts;

namespace Tickets.Logging;

public static class MarkPendingUseCase
{
    public static MarkPending MarkPending(Serilog.ILogger logger, MarkPending origin) => async args =>
    {
        Result<Ticket> pending = await origin(args);
        object[] parameters = [args.TicketId, pending.IsSuccess, pending.Error.Message];
        
        if (pending.IsFailure)
        {
            logger.Error("""
                         Перенос заявки в ожидание
                         ID: {Id}
                         Успешен: {IsSuccess}
                         Ошибка: {Error}
                         """, [parameters]);
        }
        
        if (pending.IsSuccess)
        {
            logger.Error("""
                         Перенос заявки в ожидание
                         ID: {Id}
                         Успешен: {IsSuccess}
                         """, [parameters]);
        }

        return pending;
    };

    extension(MarkPending origin)
    {
        public MarkPending WithLogging(IServiceProvider sp)
        {
            return MarkPending(sp.Resolve<ILogger>(), origin);
        }
    }
}