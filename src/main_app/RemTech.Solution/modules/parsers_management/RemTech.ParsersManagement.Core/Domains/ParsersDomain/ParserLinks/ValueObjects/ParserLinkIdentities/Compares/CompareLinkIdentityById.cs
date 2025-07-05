using RemTech.ParsersManagement.Core.Common.Primitives.Comparing;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;

public sealed class CompareLinkIdentityById : ICompare
{
    private readonly bool _compared;

    public CompareLinkIdentityById(ParserLinkIdentity link, ParserLinkIdentity other)
        : this(link.ReadId(), other.ReadId()) { }

    public CompareLinkIdentityById(IParserLink link, ParserLinkIdentity other)
        : this(link.Identification().ReadId(), other.ReadId()) { }

    public CompareLinkIdentityById(IParserLink link, IParserLink other)
        : this(link, other.Identification()) { }

    public CompareLinkIdentityById(Guid linkId, Guid relatedId) => _compared = linkId == relatedId;

    public CompareLinkIdentityById(IParserLink link, Guid id)
    {
        Guid linkId = link.Identification().ReadId().GuidValue();
        _compared = linkId == id;
    }

    public bool Equality() => _compared;

    public static implicit operator bool(CompareLinkIdentityById id) => id._compared;
}
