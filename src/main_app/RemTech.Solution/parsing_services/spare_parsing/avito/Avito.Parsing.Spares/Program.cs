using Avito.Parsing.Spares;
using Parsing.Cache;
using Parsing.RabbitMq.Configuration;
using Parsing.RabbitMq.CreateParser;
using Parsing.RabbitMq.StartParsing;
using Parsing.SDK.Browsers;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
bool isDevelopment = builder.Environment.IsDevelopment();
if (isDevelopment)
{
    BrowserFactory.DevelopmentMode();
}
else
{
    BrowserFactory.ProductionMode();
}
builder.Services.AddSingleton<Serilog.ILogger>(
    new LoggerConfiguration().WriteTo.Console().CreateLogger()
);
builder.Services.AddHostedService<Worker>();
new DisabledTrackerConfigurationSource(isDevelopment).Provide().Register(builder.Services);
new RabbitMqConfigurationSource(isDevelopment)
    .Provide()
    .Register(builder.Services, new StartParsingListenerOptions("Avito", "Запчасти"));
var host = builder.Build();
ICreateNewParserPublisher createPublisher =
    host.Services.GetRequiredService<ICreateNewParserPublisher>();
createPublisher
    .SendCreateNewParser(new CreateNewParserMessage("Avito", "Запчасти", "avito.ru"))
    .Wait();
host.Run();
