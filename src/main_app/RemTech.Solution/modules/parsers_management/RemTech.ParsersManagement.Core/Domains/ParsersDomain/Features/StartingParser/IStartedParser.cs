using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser;

public interface IStartedParser
{
    Status<IParser> Started(StartParser start);
}