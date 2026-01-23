using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicleCharacteristics;

public sealed class GetVehicleCharacteristicsQuery : IQuery
{
    public Guid? BrandId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Guid? ModelId { get; private set; }

    public GetVehicleCharacteristicsQuery ForBrand(Guid? brandId)
    {
        if (BrandId.HasValue)
            return this;
        BrandId = brandId;
        return this;
    }

    public GetVehicleCharacteristicsQuery ForCategory(Guid? categoryId)
    {
        if (CategoryId.HasValue)
            return this;
        CategoryId = categoryId;
        return this;
    }

    public GetVehicleCharacteristicsQuery ForModel(Guid? modelId)
    {
        if (ModelId.HasValue)
            return this;
        ModelId = modelId;
        return this;
    }
}
