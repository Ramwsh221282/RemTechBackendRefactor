using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser;

public sealed class DisabledParser : IDisabledParser
{
    public Status<IParser> Disable(DisableParser disable)
    {
        IParser parser = disable.Take();
        Status disabling = parser.Disable();
        return disabling.IsSuccess ? parser.Success() : disabling.Error;
    }
}
