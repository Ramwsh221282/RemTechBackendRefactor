using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Decorators;

public sealed class StatusCachingNewParser(INewParser inner) : INewParser
{
    private MaybeBag<Status<IParser>> _bag = new();

    public Status<IParser> Register(AddNewParser add)
    {
        if (_bag.Any())
            return _bag.Take();
        Status<IParser> parser = inner.Register(add);
        _bag = _bag.Put(parser);
        return parser;
    }
}
