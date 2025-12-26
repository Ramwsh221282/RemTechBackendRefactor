using ParsedAdvertisements.Core.BrandContext.ValueObjects;
using ParsedAdvertisements.Core.CategoryContext.ValueObjects;
using ParsedAdvertisements.Core.ModelContext.ValueObjects;
using ParsedAdvertisements.Core.RegionContext.ValueObjects;

namespace ParsedAdvertisements.Core.VehicleContext.ValueObjects;

public sealed record VehicleMetadata(CategoryId CategoryId, BrandId BrandId, ModelId ModelId, RegionId RegionId);