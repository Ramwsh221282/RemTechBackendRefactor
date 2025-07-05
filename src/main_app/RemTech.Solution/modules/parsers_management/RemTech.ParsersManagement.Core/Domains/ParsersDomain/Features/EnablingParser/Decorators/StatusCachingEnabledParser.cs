using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Decorators;

public sealed class StatusCachingEnabledParser(IEnabledParser inner) : IEnabledParser
{
    private MaybeBag<Status<IParser>> _bag = new();

    public Status<IParser> Enable(EnableParser enable)
    {
        if (_bag.Any())
            return _bag.Take();
        Status<IParser> status = inner.Enable(enable);
        _bag = _bag.Put(status);
        return status;
    }
}
