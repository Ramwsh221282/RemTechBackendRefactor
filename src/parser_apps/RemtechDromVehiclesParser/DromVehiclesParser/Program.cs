using DotNetEnv.Configuration;
using DromVehiclesParser.DependencyInjection;
using ParserSubscriber.SubscribtionContext;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using Serilog;
using Serilog.Core;

Logger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
logger.Information("Запуск Drom Vehicles Parser");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    bool isDevelopment = builder.Environment.IsDevelopment();
    if (isDevelopment)
    {
        if (!File.Exists("appsettings.json"))
        {
            throw new ApplicationException("Файл конфигурации appsettings.json не найден");
        }

        logger.Information("Среда разработки");
    }
    else
    {
        if (!File.Exists(".env"))
        {
            throw new ApplicationException("Файл конфигурации .env не найден");
        }

        logger.Information("Работа в продакшн среде");

        builder.Configuration.AddDotNetEnv(".env");
        builder.Configuration.AddEnvironmentVariables();
    }

    builder.Configuration.Register(builder.Services, isDevelopment);
    logger.Information("Конфигурация зарегистрирована");

    builder.Services.RegisterLogging();
    logger.Information("Логгирование зарегистрировано");

    builder.Services.RegisterParserSubscription();
    logger.Information("Подписка на парсер зарегистрирована");        

    builder.Services.RegisterDependenciesForParsing(isDevelopment);
    logger.Information("Зависимости для парсинга зарегистрированы");

    builder.Services.RegisterInfrastructureDependencies(isDevelopment);
    logger.Information("Инфраструктурные зависимости зарегистрированы");

    WebApplication app = builder.Build();

    logger.Information("Запуск подписки/повторной подписки на основной бекенд. Fire And Forget.");
    await Task.Delay(TimeSpan.FromMinutes(1));
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        _ = Task.Run(async () =>
        {
            await app.Services.RunParserSubscription();
        });
    });

    app.Services.ApplyModuleMigrations();
    logger.Information("Миграции применены");

    logger.Information("Запуск приложения");
    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Ошибка при старте приложения");
    throw;
}
finally
{
    logger.Information("Окончание работы логгера стартапа");
    await logger.DisposeAsync();
}

namespace DromVehiclesParser
{
    public partial class Program { }
}
