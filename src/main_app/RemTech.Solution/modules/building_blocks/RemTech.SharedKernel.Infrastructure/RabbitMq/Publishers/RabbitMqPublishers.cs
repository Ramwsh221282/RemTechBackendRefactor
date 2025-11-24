using RabbitMQ.Client;
using RemTech.SharedKernel.Infrastructure.RabbitMq.Publishers.Topic;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq.Publishers;

public sealed class RabbitMqPublishers
{
    private readonly RabbitMqConnectionSource _connectionSource;

    public async Task<IRabbitMqPublisher<TopicPublishOptions>> TopicPublisher(CancellationToken ct = default)
    {
        IConnection connection = await _connectionSource.GetConnection(ct);
        return new TopicRabbitMqPublisher(connection);
    }

    public RabbitMqPublishers(RabbitMqConnectionSource connectionSource)
    {
        _connectionSource = connectionSource;
    }
}