namespace Parsing.RabbitMq.CreateParser;

public interface ICreateNewParserPublisher
{
    Task<bool> SendCreateNewParser(CreateNewParserMessage message, CancellationToken ct = default);
}
