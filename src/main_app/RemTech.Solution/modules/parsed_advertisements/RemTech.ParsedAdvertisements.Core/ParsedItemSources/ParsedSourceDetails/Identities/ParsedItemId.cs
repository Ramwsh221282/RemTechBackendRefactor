using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.ParsedItemSources.ParsedSourceDetails.Identities;

public sealed record ParsedItemId
{
    private readonly NotEmptyString _id;

    public ParsedItemId(NotEmptyString id) => _id = id;

    public static implicit operator string(ParsedItemId id) => id._id;

    public static implicit operator bool(ParsedItemId id) => id._id;

    public static implicit operator NotEmptyString(ParsedItemId id) => id._id;
}
