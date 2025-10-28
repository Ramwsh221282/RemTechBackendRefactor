using ParsedAdvertisements.Domain.RegionContext.ValueObjects;

namespace ParsedAdvertisements.Domain.RegionContext;

public sealed class Region
{
    public RegionId Id { get; }
    public RegionName Name { get; }

    public Region(RegionName name, RegionId? id = null)
    {
        Id = id ?? new RegionId();
        Name = name;
    }
}
