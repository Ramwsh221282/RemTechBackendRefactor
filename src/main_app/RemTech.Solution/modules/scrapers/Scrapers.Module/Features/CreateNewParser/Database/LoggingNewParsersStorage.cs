using Scrapers.Module.Features.CreateNewParser.Models;
using Serilog;

namespace Scrapers.Module.Features.CreateNewParser.Database;

internal sealed class LoggingNewParsersStorage(ILogger logger, INewParsersStorage origin)
    : INewParsersStorage
{
    public async Task Save(NewParser parser, CancellationToken ct = default)
    {
        try
        {
            await origin.Save(parser, ct);
            logger.Information(
                "Saved new parser: {Name} {Type} {Domain}",
                parser.Name,
                parser.Type.Type,
                parser.Domain.Domain
            );
        }
        catch (Exception ex)
        {
            logger.Fatal("Exception at saving new parser. {Message}.", ex.Message);
        }
    }
}
