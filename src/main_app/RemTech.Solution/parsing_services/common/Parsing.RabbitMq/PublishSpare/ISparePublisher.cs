namespace Parsing.RabbitMq.PublishSpare;

public interface ISparePublisher
{
    Task Publish(SpareSinkMessage message);
}
