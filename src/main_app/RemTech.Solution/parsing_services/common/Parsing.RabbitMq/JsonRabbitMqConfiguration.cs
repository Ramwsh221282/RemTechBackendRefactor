using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Parsing.RabbitMq;

public sealed class JsonRabbitMqConfiguration(
    string hostName,
    string userName,
    string password,
    int port
) : IRabbitMqConfiguration
{
    public JsonRabbitMqConfiguration(string hostName, string userName, string password, string port)
        : this(hostName, userName, password, int.Parse(port)) { }

    public void Register(IServiceCollection services)
    {
        ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
            Port = port,
        };
        services.AddSingleton(factory);
        services.AddSingleton<ICreateNewParserPublisher, CreateNewParserPublisher>();
    }
}
