using Scrapers.Module.Features.DisablingParser.Exceptions;

namespace Scrapers.Module.Features.DisablingParser.Models;

internal sealed record ParserToDisable(string Name, string Type, string State)
{
    public DisabledParser Disable()
    {
        if (State == "Работает")
            throw new UnableToDisableWorkingParserException();
        if (State == "Отключен")
            throw new UnableToDisableDisabledParserException();
        if (Type == "Техника" || Type == "Запчасти")
            return new DisabledParser(Name, Type, State);
        throw new UnsupportedDisabledParserTypeException(Type);
    }
}
