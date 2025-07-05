using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.Primitives.Comparing;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;

public sealed class CompareLinkIdentityByParserOwning : ICompare
{
    private readonly bool _compared;

    public CompareLinkIdentityByParserOwning(IParserLink link, ParserIdentity identity)
        : this(link.Identification().OwnerIdentification(), identity) { }

    public CompareLinkIdentityByParserOwning(IParserLink link, IParser parser)
        : this(link.Identification().OwnerIdentification(), parser.Identification()) { }

    public CompareLinkIdentityByParserOwning(
        ParserIdentity relatedIdentity,
        ParserIdentity parserIdentity
    )
        : this(relatedIdentity.ReadId(), parserIdentity.ReadId()) { }

    public CompareLinkIdentityByParserOwning(NotEmptyGuid parserId, NotEmptyGuid relatedId)
        : this(parserId.GuidValue(), relatedId.GuidValue()) { }

    public CompareLinkIdentityByParserOwning(Guid parserId, Guid relatedId) =>
        _compared = parserId == relatedId;

    public bool Equality() => _compared;

    public static implicit operator bool(CompareLinkIdentityByParserOwning compare) =>
        compare._compared;
}
