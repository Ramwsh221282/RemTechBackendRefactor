using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using Serilog;

namespace Identity.Logging;

public static class RegisterSubjectLoggingUseCase
{
    private static RegisterSubject WithLogging(RegisterSubject origin, ILogger logger) => async args =>
    {
        Result<Subject> result = await origin(args);
        object[] properties = [args.Email, args.Login, result.IsSuccess, result.Error];
        if (result.IsFailure)
            logger.Error("""
                         Регистрация учетной записи:
                         Email: {Email}
                         Login: {Login}
                         Успех: {Success}
                         Ошибка: {Error}
                         """, properties);
        if (result.IsSuccess)
            logger.Information("""
                               Регистрация учетной записи:
                               Email: {Email}
                               Login: {Login}
                               Успех: {Success}
                               """, properties);
        return result;
    };
    
    
    extension(RegisterSubject origin)
    {
        public RegisterSubject WithLogging(IServiceProvider sp)
        {
            ILogger logger = sp.Resolve<ILogger>();
            return WithLogging(origin, logger);
        }
    }
}