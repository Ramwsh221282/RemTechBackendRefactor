using Scrapers.Module.Features.StartParser.Models;

namespace Scrapers.Module.Features.StartParser.RabbitMq;

public interface IParserStartedPublisher : IDisposable, IAsyncDisposable
{
    Task Publish(StartedParser parser);
}
