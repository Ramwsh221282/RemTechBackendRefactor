using RemTech.ParsedAdvertisements.Core.ParsedItemSources.ParsedSourceDetails.Identities;

namespace RemTech.ParsedAdvertisements.Core.ParsedItemSources.ParsedSourceDetails;

public sealed class ParsedSource(ParsedSourceIdentity identity)
{
    private readonly ParsedSourceIdentity _identity = identity;
}
