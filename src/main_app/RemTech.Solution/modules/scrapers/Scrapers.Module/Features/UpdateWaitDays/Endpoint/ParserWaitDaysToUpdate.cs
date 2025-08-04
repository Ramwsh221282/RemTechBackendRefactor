using Scrapers.Module.Features.UpdateWaitDays.Exceptions;

namespace Scrapers.Module.Features.UpdateWaitDays.Endpoint;

internal sealed record ParserWaitDaysToUpdate(
    string ParserName,
    string ParserType,
    string ParserState,
    DateTime NextRun
)
{
    public ParserWithUpdatedWaitDays Update(int newWaitDays)
    {
        if (ParserState == "Работает")
            throw new UnableToUpdateWaitDaysForWorkingParserException(ParserName, ParserType);
        if (newWaitDays < 0)
            throw new InvalidParserWaitDaysToUpdateException(newWaitDays);
        if (newWaitDays > 7)
            throw new ParserWaitDaysExceesMaxAmountException(newWaitDays, 7);
        DateTime nextRun = DateTime.UtcNow.AddDays(newWaitDays);
        return new ParserWithUpdatedWaitDays(
            ParserName,
            ParserType,
            ParserState,
            nextRun,
            newWaitDays
        );
    }
}
