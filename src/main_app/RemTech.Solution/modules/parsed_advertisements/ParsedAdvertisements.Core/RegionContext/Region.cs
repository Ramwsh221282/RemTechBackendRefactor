using ParsedAdvertisements.Core.RegionContext.ValueObjects;

namespace ParsedAdvertisements.Core.RegionContext;

public sealed record Region(RegionMetadata Metadata, RegionKind Kind);
