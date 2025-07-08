using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Comparing;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities;

public sealed class ParserLinkIdentity : ISameBy
{
    private readonly ParserIdentity _parserIdentity;
    private readonly NotEmptyGuid _id;
    private readonly Name _name;

    public ParserLinkIdentity(ParserIdentity parserIdentity, NotEmptyGuid id, Name name)
    {
        _parserIdentity = parserIdentity;
        _id = id;
        _name = name;
    }

    public ParserLinkIdentity(IParser parser, Name name)
    {
        _parserIdentity = parser.Identification();
        _name = name;
        _id = NotEmptyGuid.New();
    }

    public ParserLinkIdentity(IParser parser, NotEmptyGuid id, Name name)
    {
        _parserIdentity = parser.Identification();
        _id = id;
        _name = name;
    }

    public Name ReadName() => _name;

    public NotEmptyGuid ReadId() => _id;

    public ParserIdentity OwnerIdentification() => _parserIdentity;

    public bool SameBy(ICompare compare) => compare.Equality();
}
