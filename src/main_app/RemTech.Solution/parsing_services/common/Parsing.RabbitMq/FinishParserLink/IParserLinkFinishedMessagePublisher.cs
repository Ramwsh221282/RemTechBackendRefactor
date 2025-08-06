namespace Parsing.RabbitMq.FinishParserLink;

public interface IParserLinkFinishedMessagePublisher
{
    Task Publish(FinishedParserLinkMessage message);
}
