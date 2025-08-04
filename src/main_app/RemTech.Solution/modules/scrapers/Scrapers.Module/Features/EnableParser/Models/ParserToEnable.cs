using Scrapers.Module.Features.EnableParser.Exceptions;

namespace Scrapers.Module.Features.EnableParser.Models;

internal sealed record ParserToEnable(string Name, string State, string Type)
{
    public EnabledParser Enable()
    {
        if (State == "Работает")
            throw new UnableToEnableParserWhenWorkingException();
        if (State == "Ожидает")
            throw new UnableToEnableWaitingParserException();
        if (Type == "Техника" || Type == "Запчасти")
            return new EnabledParser(Name, State, Type);
        throw new UnsupportedEnabledParserTypeException(Type);
    }
}
