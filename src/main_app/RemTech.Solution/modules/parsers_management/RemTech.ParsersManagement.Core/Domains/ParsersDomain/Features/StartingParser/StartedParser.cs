using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser;

public sealed class StartedParser : IStartedParser
{
    public Status<IParser> Started(StartParser start)
    {
        IParser parser = start.TakeStarter();
        Status starting = parser.Start();
        return starting.IsSuccess ? parser.Success() : starting.Error;
    }
}
