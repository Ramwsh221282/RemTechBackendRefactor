using Scrapers.Module.Features.DisablingParser.Models;

namespace Scrapers.Module.Features.DisablingParser.Database;

internal interface IDisabledParsersStorage
{
    Task<DisabledParser> SaveAsync(DisabledParser parser, CancellationToken ct = default);
}
