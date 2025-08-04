using Scrapers.Module.Features.DisablingParser.Database;
using Scrapers.Module.Features.DisablingParser.Models;

namespace Scrapers.Module.Features.DisablingParser.Logging;

internal sealed class LoggingDisabledParsersStorage(
    Serilog.ILogger logger,
    IDisabledParsersStorage storage
) : IDisabledParsersStorage
{
    public async Task<DisabledParser> SaveAsync(
        DisabledParser parser,
        CancellationToken ct = default
    )
    {
        try
        {
            DisabledParser disabled = await storage.SaveAsync(parser, ct);
            logger.Information(
                "Парсер {Name} {Type} {State} отключен.",
                parser.Name,
                parser.Type,
                parser.State
            );
            return disabled;
        }
        catch (UnableToDisableDisabledParserException ex)
        {
            logger.Fatal("{Ex}", ex.Message);
            throw;
        }
        catch (UnableToDisableWorkingParserException ex)
        {
            logger.Fatal("{Ex}", ex.Message);
            throw;
        }
        catch (UnableToFindParserToDisableException ex)
        {
            logger.Fatal("{Ex}", ex.Message);
            throw;
        }
        catch (UnsupportedDisabledParserTypeException ex)
        {
            logger.Fatal("{Ex}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            logger.Fatal("{Ex}", ex.Message);
            throw;
        }
    }
}
