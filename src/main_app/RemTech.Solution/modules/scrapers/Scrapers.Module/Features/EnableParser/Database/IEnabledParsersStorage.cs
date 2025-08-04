using Scrapers.Module.Features.EnableParser.Models;

namespace Scrapers.Module.Features.EnableParser.Database;

internal interface IEnabledParsersStorage
{
    Task<EnabledParser> Save(EnabledParser parser, CancellationToken cancellationToken = default);
}
