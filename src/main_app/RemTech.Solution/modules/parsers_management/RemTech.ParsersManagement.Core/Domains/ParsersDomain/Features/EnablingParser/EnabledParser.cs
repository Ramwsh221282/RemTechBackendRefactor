using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser;

public sealed class EnabledParser : IEnabledParser
{
    public Status<IParser> Enable(EnableParser enable)
    {
        IParser parser = enable.WhomEnable();
        Status enabling = parser.Enable();
        return enabling.IsSuccess ? parser.Success() : enabling.Error;
    }
}
