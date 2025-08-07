using Scrapers.Module.Features.StartParser.Models;

namespace Scrapers.Module.Features.StartParser.RabbitMq;

internal interface IParserStartedPublisher : IDisposable, IAsyncDisposable
{
    Task Publish(StartedParser parser);
}
