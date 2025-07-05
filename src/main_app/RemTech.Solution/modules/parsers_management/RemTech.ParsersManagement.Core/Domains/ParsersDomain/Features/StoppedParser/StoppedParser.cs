using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser;

public sealed class StoppedParser : IStoppedParser
{
    public Status<IParser> Stopped(StopParser stop)
    {
        IParser parser = stop.WhomStop();
        Status stopping = parser.Stop();
        return stopping.IsSuccess ? parser.Success() : stopping.Error;
    }
}
