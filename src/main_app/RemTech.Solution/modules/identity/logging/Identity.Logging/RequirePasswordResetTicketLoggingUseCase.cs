using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Tickets;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Logging;

public static class RequirePasswordResetTicketLoggingUseCase
{
    private static RequirePasswordResetTicket WithLogging(Serilog.ILogger logger, RequirePasswordResetTicket origin) => async args =>
    {
        Result<SubjectTicket> result = await origin(args);
        object[] parameters = [args.SubjectId, result.IsSuccess, result.Error.Message];
        
        if (result.IsFailure)
        {
            logger.Information(
                """ 
                Создана заявка на сброс пароля:
                Subject ID: {Id}
                Успешно: {IsSuccess}
                Ошибка: {Error}
                """, parameters);
        }

        if (result.IsSuccess)
        {
            logger.Information(
                """ 
                Создана заявка на сброс пароля:
                Subject ID: {Id}
                Успешно: {IsSuccess}
                """, parameters);
        }

        return result;
    };

    extension(RequirePasswordResetTicket origin)
    {
        public RequirePasswordResetTicket WithLogging(IServiceProvider sp)
        {
            Serilog.ILogger logger = sp.Resolve<Serilog.ILogger>();
            return WithLogging(logger, origin);
        }
    }
}