using Drom.Parsing.Vehicles;
using Parsing.RabbitMq.Configuration;
using Parsing.RabbitMq.CreateParser;
using Parsing.RabbitMq.StartParsing;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
bool isDevelopment = builder.Environment.IsDevelopment();
builder.Services.AddSingleton<Serilog.ILogger>(
    new LoggerConfiguration().WriteTo.Console().CreateLogger()
);
builder.Services.AddHostedService<Worker>();
if (isDevelopment)
{
    string file = "appsettings.json";
    IRabbitMqConfigurationSource configSource = new JsonRabbitMqConfigurationSource(file);
    configSource
        .Provide()
        .Register(builder.Services, new StartParsingListenerOptions("Drom", "Техника"));
}
var host = builder.Build();
ICreateNewParserPublisher createPublisher =
    host.Services.GetRequiredService<ICreateNewParserPublisher>();
bool createSended = createPublisher
    .SendCreateNewParser(new CreateNewParserMessage("Drom", "Техника", "drom.ru"))
    .Result;
host.Run();
