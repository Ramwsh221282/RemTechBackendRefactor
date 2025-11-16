using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using Serilog;

namespace Identity.Logging;

public static class AuthorizeSubjectLogging
{
    private static AuthorizeSubject AuthorizeSubject(ILogger logger, AuthorizeSubject origin) => async args =>
    {
        Result<Subject> result = await origin(args);

        string login = args.Login.HasValue ? args.Login.Value : "NO_LOGIN_PROVIDED";
        string email = args.Email.HasValue ? args.Email.Value : "NO_EMAIL_PROVIDED";
        object[] parameters = [login, email, result.IsSuccess, result.Error.Message];
        
        if (result.IsSuccess)
        {
            logger.Information(
                """
                Авторизация:
                Login: {Login}
                Email: {Email}
                Успешно: {IsSuccess}
                """, parameters);
        }
        
        if (result.IsFailure)
        {
            logger.Error(
                """
                Авторизация:
                Login: {Login}
                Email: {Email}
                Успешно: {IsSuccess}
                Ошибка: {Error}
                """, parameters);
        }

        return result;
    };

    extension(AuthorizeSubject origin)
    {
        public AuthorizeSubject WithLogging(IServiceProvider sp)
        {
            ILogger logger = sp.Resolve<ILogger>();
            return AuthorizeSubject(logger, origin);
        }
    }
}