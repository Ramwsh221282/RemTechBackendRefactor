using Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;

namespace Scrapers.Module.Features.ReadConcreteScraper.Storage;

internal interface IConcreteScraperStorage
{
    Task<ParserResult?> Read(string name, string type, CancellationToken ct = default);
}
