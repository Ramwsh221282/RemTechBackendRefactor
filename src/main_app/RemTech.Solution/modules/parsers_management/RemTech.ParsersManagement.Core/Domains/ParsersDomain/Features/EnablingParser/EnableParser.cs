using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser;

public sealed class EnableParser
{
    private MaybeBag<IParser> _bag;

    public EnableParser() => _bag = new MaybeBag<IParser>();

    public EnableParser(IParser parser) => _bag = new MaybeBag<IParser>(parser);

    public void PutParser(IParser parser)
    {
        if (_bag.Any())
            return;
        _bag = _bag.Put(parser);
    }

    public IParser WhomEnable() => _bag.Take();
}
