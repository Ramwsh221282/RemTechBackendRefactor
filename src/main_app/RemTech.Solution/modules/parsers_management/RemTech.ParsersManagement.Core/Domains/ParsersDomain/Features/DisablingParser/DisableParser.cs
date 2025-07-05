using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser;

public sealed class DisableParser : IMaybeParser
{
    private readonly ParserBag _bag;

    public DisableParser() => _bag = new ParserBag();

    public DisableParser(IParser parser) => _bag = new ParserBag(parser);

    public void Put(IParser parser) => _bag.Put(parser);

    public IParser Take() => _bag.Take();
}
