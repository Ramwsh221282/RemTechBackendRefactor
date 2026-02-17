using DotNetEnv.Configuration;
using ParserSubscriber.SubscribtionContext;
using ParsingSDK.ParserStopingContext;
using Quartz;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.Quartz;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTechAvitoVehiclesParser;
using RemTechAvitoVehiclesParser.RabbitMq.Consumers;
using Serilog;
using Serilog.Core;

Logger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
logger.Information("Запуск RemTech Avito Vehicles Parser...");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    bool isDevelopment = builder.Environment.IsDevelopment();
    if (isDevelopment)
    {
        if (!File.Exists("appsettings.json"))
        {
            throw new FileNotFoundException("Файл конфигурации appsettings.json не найден.");
        }

        logger.Information("Используется среда разработки.");
    }
    else
    {
        if (!File.Exists(".env"))
        {
            throw new FileNotFoundException("Файл конфигурации .env не найден.");
        }

        logger.Information("Используется среда продакшна.");
        builder.Configuration.AddDotNetEnv(".env");
        builder.Configuration.AddEnvironmentVariables();
    }

    builder.Configuration.Register(builder.Services, isDevelopment);
    logger.Information("Конфигурация зарегистрирована.");

    builder.Services.RegisterLogging();
    logger.Information("Логгер зарегистрирован.");

    builder.Services.RegisterDependenciesForParsing(isDevelopment);
    logger.Information("Зависимости для парсинга зарегистрированы.");

    builder.Services.RegisterInfrastructureDependencies(isDevelopment);
    logger.Information("Инфраструктурные зависимости зарегистрированы.");

    builder.Services.AddCronScheduledJobs();
    logger.Information("Планировщик заданий (бекграунд процессы) зарегистрирован.");

    builder.Services.AddTransient<IConsumer, ParserStartConsumer>();
    builder.Services.AddTransient<IConsumer, ParserStopConsumer>();
    builder.Services.AddHostedService<AggregatedConsumersHostedService>();
    logger.Information("RabbitMQ потребители зарегистрированы.");

    builder.Services.AddQuartzHostedService(c =>
    {
        c.WaitForJobsToComplete = true;
        c.StartDelay = TimeSpan.FromSeconds(10);
    });

    WebApplication app = builder.Build();

    logger.Information(
        "Попытка/повторная попытка подписки в основной бекенд запущена Fire And Forget."
    );
    await Task.Delay(TimeSpan.FromMinutes(1));
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        _ = Task.Run(async () =>
        {
            await app.Services.RunParserSubscription();
        });
    });

    if (args.Contains("--rollback-migrations"))
    {
        app.Services.RollBackModuleMigrations();
    }

    app.Services.ApplyModuleMigrations();
    logger.Information("Миграции применены.");

    logger.Information("Запуск приложения.");
    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Приложение завершилось с фатальной ошибкой.");
    throw;
}
finally
{
    logger.Information("Остановка логгера стартапа.");
    await logger.DisposeAsync();
}

namespace RemTechAvitoVehiclesParser
{
    public partial class Program { }
}
