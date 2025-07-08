using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsersManagement.External.ParsedItems.ParsedContainedItems.ValueObjects;

public sealed record ContainedItemSourceInfo
{
    private readonly NotEmptyGuid _linkId;
    private readonly NotEmptyString _linkName;
    private readonly NotEmptyString _linkUrl;

    public ContainedItemSourceInfo(
        NotEmptyGuid linkId,
        NotEmptyString linkName,
        NotEmptyString linkUrl
    )
    {
        _linkId = linkId;
        _linkName = linkName;
        _linkUrl = linkUrl;
    }

    public ContainedItemSourceInfo(Guid? linkId, string? linkName, string? linkUrl)
        : this(new NotEmptyGuid(linkId), new NotEmptyString(linkName), new NotEmptyString(linkUrl))
    { }

    public static implicit operator bool(ContainedItemSourceInfo sourceInfo) =>
        sourceInfo._linkId && sourceInfo._linkName && sourceInfo._linkUrl;
}
