using Drom.Parsing.Vehicles;
using Parsing.Cache;
using Parsing.Grpc.Services.DuplicateIds;
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
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<Serilog.ILogger>(
    new LoggerConfiguration().WriteTo.Console().CreateLogger()
);
builder.Services.AddHostedService<Worker>();
DuplicateIdsCheckClientOptions.Create(isDevelopment).Register(builder.Services);
new DisabledTrackerConfigurationSource(isDevelopment).Provide().Register(builder.Services);
new RabbitMqConfigurationSource(isDevelopment)
    .Provide()
    .Register(builder.Services, new StartParsingListenerOptions("Drom", "Техника"));
var host = builder.Build();
ICreateNewParserPublisher createPublisher =
    host.Services.GetRequiredService<ICreateNewParserPublisher>();
bool createSended = createPublisher
    .SendCreateNewParser(new CreateNewParserMessage("Drom", "Техника", "drom.ru"))
    .Result;
host.Run();
