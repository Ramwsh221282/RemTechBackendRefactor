using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Decorators;

public sealed class StatusCachingDisabledParser(IDisabledParser inner) : IDisabledParser
{
    private MaybeBag<Status<IParser>> _bag = new();

    public Status<IParser> Disable(DisableParser disable)
    {
        if (_bag.Any())
            return _bag.Take();
        Status<IParser> parser = inner.Disable(disable);
        _bag = _bag.Put(parser);
        return parser;
    }
}
