using RemTech.ParsersManagement.Core.Common.Primitives.Comparing;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkActivities.Compares;

public sealed class CompareLinkActivity : ICompare
{
    private readonly bool _compare;

    public CompareLinkActivity(IParserLink link, bool comparison)
        : this(link.Activity().Read(), comparison) { }

    public CompareLinkActivity(bool linkActivity, bool comparison) =>
        _compare = linkActivity == comparison;

    public bool Equality() => _compare;

    public static implicit operator bool(CompareLinkActivity compare) => compare._compare;
}
