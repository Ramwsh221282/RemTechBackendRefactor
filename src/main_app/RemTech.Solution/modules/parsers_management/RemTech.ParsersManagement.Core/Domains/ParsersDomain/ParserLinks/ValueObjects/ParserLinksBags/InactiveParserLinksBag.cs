namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;

public sealed class InactiveParserLinksBag
{
    private readonly bool _empty;

    public InactiveParserLinksBag(ParserLinksBag bag) =>
        _empty = bag.Read().All(l => l.Activity() == false);

    public bool Any() => _empty;
}
