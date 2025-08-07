namespace Parsing.RabbitMq.FinishParser;

public interface IParserFinishMessagePublisher
{
    Task Publish(ParserFinishedMessage message);
}
