using RemTech.Core.Shared.Exceptions;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.QueryModifiers;

public sealed class BrandedVehiclePresentMod : IVehiclePresentQueryMod
{
    private readonly Guid _brandId;

    public BrandedVehiclePresentMod(Guid brandId) => _brandId = brandId;
    
    public VehiclePresentQueryStorage Modified(VehiclePresentQueryStorage storage)
    {
        return _brandId == Guid.Empty
            ? throw new OperationException("Невозможно выполнить запрос. Идентификатор бренд техники пустой.")
            : storage.Put($"v.brand_id = @brand_id", "@brand_id", _brandId);
    }
}