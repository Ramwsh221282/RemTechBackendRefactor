using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using Serilog;

namespace Identity.Logging;

public static class ChangeEmailLoggingUseCase
{
    internal static ChangeEmail ChangeEmailLogging(ChangeEmail origin, ILogger logger) => async args =>
    {
        Result<Subject> operation = await origin(args);
        if (operation.IsFailure)
        {
            object[] properties = [args.Id, args.NextEmail, false, operation.Error.Message];
            logger.Error("""
                         Id: {Id}
                         Изменяет Email на: {Email}
                         Успешно: {Success}
                         Ошибка: {Error}
                         """, properties);
            return operation;
        }

        object[] successProperties = [args.Id, args.NextEmail, true];
        logger.Information("""
                           ID: {Id}
                           Изменяет Email на {Email}
                           Успешно: {Success} 
                           """, successProperties);
        return operation;
    };

    extension(ChangeEmail origin)
    {
        public ChangeEmail WithLogging(IServiceProvider sp)
        {
            return ChangeEmailLogging(origin, sp.Resolve<ILogger>());
        }
    }
}