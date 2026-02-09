using AvitoSparesParser.Extensions;
using AvitoSparesParser.RabbitMq.Consumers;
using DotNetEnv.Configuration;
using Microsoft.Extensions.Options;
using ParserSubscriber.SubscribtionContext;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Serilog;
using Serilog.Core;

Logger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
logger.Information("Запуск парсера автозапчастей AvitoSparesParser...");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    bool isDevelopment = builder.Environment.IsDevelopment();
    logger.Information("Продакшн - {IsProduction}", !isDevelopment);

    if (!isDevelopment)
    {
        const string envFile = ".env";
        if (!File.Exists(envFile))
        {
            logger.Fatal("Файл конфигурации окружения '{EnvFile}' отсутствует.", envFile);
            throw new ApplicationException($"Файл конфигурации окружения '{envFile}' отсутствует.");
        }

        builder.Configuration.AddDotNetEnv(envFile);
        builder.Configuration.AddEnvironmentVariables();
        logger.Information("Файл конфигурации окружения '{EnvFile}' загружен.", envFile);
    }

    builder.Configuration.Register(builder.Services, isDevelopment);
    logger.Information("Конфигурация зарегистрирована.");

    builder.Services.RegisterLogging();
    logger.Information("Логгер настроен.");

    builder.Services.RegisterDependenciesForParsing(isDevelopment);
    logger.Information("Зависимости для парсинга зарегистрированы.");

    builder.Services.AddTransient<IConsumer, StartParserConsumer>();
    builder.Services.AddHostedService<AggregatedConsumersHostedService>();
    logger.Information("Регистрация сервиса агрегированных слушателей RabbitMQ...");

    builder.Services.RegisterInfrastructureDependencies();
    logger.Information("Инфраструктурные зависимости зарегистрированы.");

    WebApplication app = builder.Build();
    
    logger.Information("Применение миграций модулей базы данных...");
    app.Services.ApplyModuleMigrations();
    logger.Information("Миграции модулей базы данных применены.");
    
    logger.Information("Попытка подписки на основной бекенд.");
    await Task.Delay(TimeSpan.FromMinutes(1));
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        _ = Task.Run(async () =>
        {
            await app.Services.RunParserSubscription();
        });
    });
    
    logger.Information("Запуск парсера автозапчастей AvitoSparesParser...");
    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(
        ex,
        "Необработанное исключение при запуске парсера автозапчастей AvitoSparesParser."
    );
    throw;
}
finally
{
    logger.Information("Выключение логгера запуска парсера автозапчастей AvitoSparesParser...");
    await logger.DisposeAsync();
}

namespace AvitoSparesParser
{
    public partial class Program { }
}
