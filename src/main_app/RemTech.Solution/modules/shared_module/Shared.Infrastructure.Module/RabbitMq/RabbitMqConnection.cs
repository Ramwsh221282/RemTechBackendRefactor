using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RemTech.Shared.Configuration;
using RemTech.Shared.Configuration.Options;

namespace Shared.Infrastructure.Module.RabbitMq;

public sealed class RabbitMqConnection
{
    public IConnection Connection { get; }

    public RabbitMqConnection(IOptions<RabbitMqOptions> options)
    {
        RabbitMqOptions rabbit = options.Value;

        ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = rabbit.HostName,
            UserName = rabbit.UserName,
            Password = rabbit.Password,
            Port = int.Parse(rabbit.Port),
        };

        Connection = factory.CreateConnectionAsync().Result;
    }
}