using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Comparing;
using RemTech.ParsersManagement.Core.Common.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;

public sealed class CompareLinkIdentityByName : ICompare
{
    private readonly bool _compared;

    public CompareLinkIdentityByName(NotEmptyString linkName, NotEmptyString other)
        : this(linkName.StringValue(), other.StringValue()) { }

    public CompareLinkIdentityByName(IParserLink link, NotEmptyString other)
        : this(link, other.StringValue()) { }

    public CompareLinkIdentityByName(IParserLink link, ParserLinkIdentity other)
        : this(link, other.ReadName()) { }

    public CompareLinkIdentityByName(IParserLink link, IParserLink related)
        : this(link, related.Identification().ReadName()) { }

    public CompareLinkIdentityByName(IParserLink link, Name name)
        : this(link.Identification(), name.NameString()) { }

    public CompareLinkIdentityByName(IParserLink link, string name)
        : this(link.Identification(), name) { }

    public CompareLinkIdentityByName(ParserLinkIdentity identity, string relatedName)
        : this(identity.ReadName().NameString(), relatedName) { }

    public CompareLinkIdentityByName(string linkName, string relatedName) =>
        _compared = linkName == relatedName;

    public bool Equality() => _compared;

    public static implicit operator bool(CompareLinkIdentityByName compare) => compare._compared;
}
