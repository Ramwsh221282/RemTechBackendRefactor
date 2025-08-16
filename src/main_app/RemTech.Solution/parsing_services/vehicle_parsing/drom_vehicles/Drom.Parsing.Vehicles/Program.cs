using Drom.Parsing.Vehicles;
using Parsing.Cache;
using Parsing.RabbitMq.Configuration;
using Parsing.RabbitMq.CreateParser;
using Parsing.RabbitMq.StartParsing;
using Parsing.SDK.Browsers;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
if (builder.Environment.IsDevelopment())
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
new DisabledTrackerConfigurationSource().Provide().Register(builder.Services);
new RabbitMqConfigurationSource()
    .Provide()
    .Register(builder.Services, new StartParsingListenerOptions("Drom", "Техника"));
var host = builder.Build();
ICreateNewParserPublisher createPublisher =
    host.Services.GetRequiredService<ICreateNewParserPublisher>();
bool createSended = createPublisher
    .SendCreateNewParser(new CreateNewParserMessage("Drom", "Техника", "drom.ru"))
    .Result;
host.Run();
