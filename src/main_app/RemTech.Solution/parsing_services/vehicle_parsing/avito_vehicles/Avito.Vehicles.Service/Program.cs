using Avito.Vehicles.Service;
using Parsing.RabbitMq.Configuration;
using Parsing.RabbitMq.CreateParser;
using Parsing.RabbitMq.StartParsing;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<Serilog.ILogger>(
    new LoggerConfiguration().WriteTo.Console().CreateLogger()
);
IRabbitMqConfigurationSource configSource = new JsonRabbitMqConfigurationSource("appsettings.json");
IRabbitMqConfiguration config = configSource.Provide();
config.Register(builder.Services, new StartParsingListenerOptions("Avito", "Техника"));
var host = builder.Build();
ICreateNewParserPublisher createPublisher =
    host.Services.GetRequiredService<ICreateNewParserPublisher>();
createPublisher
    .SendCreateNewParser(new CreateNewParserMessage("Avito", "Техника", "avito.ru"))
    .Wait();
host.Run();
