using Scrapers.Module.Features.EnableParser.Database;
using Scrapers.Module.Features.EnableParser.Exceptions;
using Scrapers.Module.Features.EnableParser.Models;

namespace Scrapers.Module.Features.EnableParser.Logging;

internal sealed class LoggingEnabledParsersStorage(
    Serilog.ILogger logger,
    IEnabledParsersStorage origin
) : IEnabledParsersStorage
{
    public async Task<EnabledParser> Save(
        EnabledParser parser,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            EnabledParser enabled = await origin.Save(parser, cancellationToken);
            logger.Information(
                "Парсер {Name} {Type} {State} включен.",
                enabled.Name,
                enabled.Type,
                enabled.State
            );
            return enabled;
        }
        catch (ParserToEnableWasNotFoundException ex)
        {
            logger.Fatal("{Ex}", ex.Message);
            throw;
        }
        catch (UnableToEnableParserWhenWorkingException ex)
        {
            logger.Fatal("{Ex}", ex.Message);
            throw;
        }
        catch (UnableToEnableWaitingParserException ex)
        {
            logger.Fatal("{Ex}", ex.Message);
            throw;
        }
        catch (UnsupportedEnabledParserTypeException ex)
        {
            logger.Fatal("{Ex}", ex.Message);
            throw;
        }
    }
}
