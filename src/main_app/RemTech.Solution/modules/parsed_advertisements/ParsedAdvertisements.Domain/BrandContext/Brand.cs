using ParsedAdvertisements.Domain.BrandContext.ValueObjects;

namespace ParsedAdvertisements.Domain.BrandContext;

public sealed class Brand
{
    public BrandId Id { get; }
    public BrandName Name { get; }

    public Brand(BrandName name, BrandId? id = null)
    {
        Name = name;
        Id = id ?? new BrandId();
    }
}
