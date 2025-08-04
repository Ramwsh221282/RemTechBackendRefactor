using Scrapers.Module.Features.CreateNewParser.Models;

namespace Scrapers.Module.Features.CreateNewParser.Cache;

internal interface ICachedNewParsers
{
    Task Save(NewParser parser, CancellationToken ct = default);
}
