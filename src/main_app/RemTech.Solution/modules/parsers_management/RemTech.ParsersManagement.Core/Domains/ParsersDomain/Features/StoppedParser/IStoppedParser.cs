using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser;

public interface IStoppedParser
{
    public Status<IParser> Stopped(StopParser stop);
}