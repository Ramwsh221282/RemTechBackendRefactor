using Identity.Core.PermissionsModule;
using Identity.Core.PermissionsModule.Contracts;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Functional.Extensions;
using Serilog;

namespace Identity.Logging;

public static class RegisterPermissionLoggingUseCase
{
    internal static RegisterPermission RegisterPermissionUseCase(
        ILogger logger,
        RegisterPermission origin) =>
        async args =>
        {
            Result<Permission> permission = await origin(args);
            object[] properties = [args.Name, permission.IsSuccess, permission.Error];
            if (permission.IsFailure)
                logger.Error(
                    """
                    Регистрация разрешения:
                    Name: {Name}
                    Успешен: {Success}
                    Ошибка: {Error}
                    """, properties);
            if (permission.IsSuccess)
                logger.Error(
                    """
                    Регистрация разрешения:
                    Name: {Name}
                    Успешен: {Success}
                    """, properties);
            return permission;
        };

    extension(RegisterPermission origin)
    {
        public RegisterPermission WithLogging(ILogger logger)
        {
            return RegisterPermissionUseCase(logger, origin);
        }
    }

    extension(IServiceProvider sp)
    {
        public RegisterPermission WithLogging(RegisterPermission origin)
        {
            ILogger logger = sp.GetRequiredService<ILogger>();
            return origin.WithLogging(logger);
        }
    }
}