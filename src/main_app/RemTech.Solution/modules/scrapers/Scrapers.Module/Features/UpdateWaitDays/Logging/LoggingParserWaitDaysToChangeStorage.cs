using Scrapers.Module.Features.UpdateWaitDays.Database;
using Scrapers.Module.Features.UpdateWaitDays.Exceptions;
using Scrapers.Module.Features.UpdateWaitDays.Models;

namespace Scrapers.Module.Features.UpdateWaitDays.Logging;

internal sealed class LoggingParserWaitDaysToChangeStorage(
    Serilog.ILogger logger,
    IParserWaitDaysToUpdateStorage origin
) : IParserWaitDaysToUpdateStorage
{
    public async Task<ParserWaitDaysToUpdate> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        try
        {
            return await origin.Fetch(parserName, parserType, ct);
        }
        catch (ParserToUpdateWaitDaysNotFoundException ex)
        {
            logger.Error("{Ex}", ex.Message);
            throw;
        }
    }

    public async Task<ParserWithUpdatedWaitDays> Save(
        ParserWithUpdatedWaitDays parser,
        CancellationToken ct = default
    )
    {
        try
        {
            ParserWithUpdatedWaitDays changed = await origin.Save(parser, ct);
            logger.Information(
                "Изменено дни ожидания {Name} {Type} {Days}",
                parser.ParserName,
                parser.ParserType,
                parser.WaitDays
            );
            return changed;
        }
        catch (ParserToUpdateWaitDaysNotFoundException ex)
        {
            logger.Error("{Ex}", ex.Message);
            throw;
        }
    }
}
