namespace Parsing.RabbitMq;

public interface ICreateNewParserPublisher
{
    Task<bool> SendCreateNewParser(CreateNewParserMessage message, CancellationToken ct = default);
}
