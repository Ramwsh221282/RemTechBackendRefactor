using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;

public sealed class ParserBag : IMaybeParser
{
    private MaybeBag<IParser> _bag;

    public ParserBag() => _bag = new MaybeBag<IParser>();

    public ParserBag(IParser parser) => _bag = new MaybeBag<IParser>(parser);

    public void Put(IParser parser)
    {
        if (_bag.Any())
            return;
        _bag = _bag.Put(parser);
    }

    public IParser Take() => _bag.Take();

    public bool Has() => _bag.Any();
}
