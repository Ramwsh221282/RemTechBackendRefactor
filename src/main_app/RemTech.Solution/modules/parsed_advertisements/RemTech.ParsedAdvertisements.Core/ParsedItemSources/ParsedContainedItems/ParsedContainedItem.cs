using RemTech.ParsedAdvertisements.Core.ParsedItemSources.ParsedSourceDetails.Identities;

namespace RemTech.ParsedAdvertisements.Core.ParsedItemSources.ParsedContainedItems;

public sealed class ParsedContainedItem
{
    private readonly ParsedContainedItemId _id;
    private readonly ParsedSourceServiceLinkIdentity _linkIdentity;

    public ParsedContainedItem(
        ParsedContainedItemId id,
        ParsedSourceServiceLinkIdentity linkIdentity
    )
    {
        _id = id;
        _linkIdentity = linkIdentity;
    }
}
