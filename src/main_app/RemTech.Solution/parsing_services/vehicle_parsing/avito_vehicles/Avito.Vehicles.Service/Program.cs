using Avito.Vehicles.Service;
using Parsing.RabbitMq.Configuration;
using Parsing.RabbitMq.CreateParser;
using Parsing.RabbitMq.StartParsing;
using Parsing.Vehicles.Grpc.Recognition;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
bool isDevelopment = builder.Environment.IsDevelopment();
builder.Services.AddSingleton<Serilog.ILogger>(
    new LoggerConfiguration().WriteTo.Console().CreateLogger()
);
builder.Services.AddHostedService<Worker>();
if (isDevelopment)
{
    string file = "appsettings.json";
    ICommunicationChannelOptionsSource communicationChannelOptions =
        new JsonCommunicationChannelOptionsSource(file);
    communicationChannelOptions.Provide().Register(builder.Services);
    IRabbitMqConfigurationSource configSource = new JsonRabbitMqConfigurationSource(file);
    IRabbitMqConfiguration config = configSource.Provide();
    config.Register(builder.Services, new StartParsingListenerOptions("Avito", "Техника"));
}
var host = builder.Build();
ICreateNewParserPublisher createPublisher =
    host.Services.GetRequiredService<ICreateNewParserPublisher>();
createPublisher
    .SendCreateNewParser(new CreateNewParserMessage("Avito", "Техника", "avito.ru"))
    .Wait();
host.Run();
