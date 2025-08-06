using Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;

namespace Scrapers.Module.Features.ReadAllTransportParsers.Storage;

internal interface IAllTransportParsersStorage
{
    Task<IEnumerable<ParserResult>> Read(CancellationToken ct = default);
}
