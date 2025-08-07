using Microsoft.Extensions.DependencyInjection;
using Parsing.RabbitMq.CreateParser;
using Parsing.RabbitMq.Facade;
using Parsing.RabbitMq.FinishParser;
using Parsing.RabbitMq.FinishParserLink;
using Parsing.RabbitMq.PublishVehicle;
using Parsing.RabbitMq.StartParsing;
using RabbitMQ.Client;

namespace Parsing.RabbitMq.Configuration;

public sealed class JsonRabbitMqConfiguration(
    string hostName,
    string userName,
    string password,
    int port
) : IRabbitMqConfiguration
{
    public JsonRabbitMqConfiguration(string hostName, string userName, string password, string port)
        : this(hostName, userName, password, int.Parse(port)) { }

    public void Register(IServiceCollection services, StartParsingListenerOptions options)
    {
        ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
            Port = port,
        };
        services.AddSingleton(factory);
        IConnection connection = factory.CreateConnectionAsync().Result;
        IChannel channel = connection.CreateChannelAsync().Result;
        channel
            .ExchangeDeclareAsync(
                RabbitMqScraperConstants.ScrapersExchange,
                ExchangeType.Direct,
                false,
                false
            )
            .Wait();
        services.AddSingleton(connection);
        services.AddSingleton(options);
        services.AddSingleton<IPublishVehiclePublisher, PublishVehiclePublisher>();
        services.AddSingleton<ICreateNewParserPublisher, CreateNewParserPublisher>();
        services.AddSingleton<IStartParsingListener, StartParsingListener>();
        services.AddSingleton<IParserFinishMessagePublisher, ParserFinishMessagePublisher>();
        services.AddSingleton<
            IParserLinkFinishedMessagePublisher,
            ParserLinkFinishedMessagePublisher
        >();
        services.AddSingleton<IParserRabbitMqActionsPublisher, ParserRabbitMqActionsPublisher>();
    }
}
