using Avito.Vehicles.Service;
using Parsing.RabbitMq;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
IRabbitMqConfigurationSource configSource = new JsonRabbitMqConfigurationSource("appsettings.json");
IRabbitMqConfiguration config = configSource.Provide();
config.Register(builder.Services);
var host = builder.Build();
ICreateNewParserPublisher createPublisher =
    host.Services.GetRequiredService<ICreateNewParserPublisher>();
createPublisher
    .SendCreateNewParser(new CreateNewParserMessage("Avito", "Техника", "avito.ru"))
    .Wait();
host.Run();
