using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using Serilog;

namespace Identity.Logging;

public static class ChangePasswordLoggingUseCase
{
    internal static ChangePassword ChangePasswordLogging(ChangePassword origin, ILogger logger) =>
        async args =>
        {
            Result<Subject> operation = await origin(args);
            if (operation.IsFailure)
            {
                object[] properties = [args.Id, operation.IsSuccess, operation.Error.Message];
                logger.Error
                ("""
                 Id: {Id}
                 Изменил пароль: {Success}
                 Ошибка: {Error}
                 """, properties);
                return operation;
            }
            
            object[] successProperties = [args.Id, operation.IsSuccess, operation.Error.Message];
            logger.Information 
            ("""
             Id: {Id}
             Изменил пароль: {Success}
             Ошибка: {Error}
             """, successProperties);
            return operation;
        };

    extension(ChangePassword origin)
    {
        public ChangePassword WithLogging(IServiceProvider sp)
        {
            return ChangePasswordLogging(origin, sp.Resolve<ILogger>());
        }
    }
}