using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser;

public interface IDisabledParser
{
    Status<IParser> Disable(DisableParser disable);
}
