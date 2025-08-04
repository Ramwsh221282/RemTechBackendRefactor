namespace Scrapers.Module.Features.EnableParser.Models;

internal interface IEnabledParsersStorage
{
    Task<EnabledParser> Save(EnabledParser parser, CancellationToken cancellationToken = default);
}
