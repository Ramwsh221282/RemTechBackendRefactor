using RemTech.Core.Shared.Exceptions;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.QueryModifiers;

public sealed class KindedVehiclePresentMod : IVehiclePresentQueryMod
{
    private readonly Guid _kindId;

    public KindedVehiclePresentMod(Guid kindId) => _kindId = kindId;
    
    public VehiclePresentQueryStorage Modified(VehiclePresentQueryStorage storage)
    {
        return _kindId == Guid.Empty
            ? throw new OperationException("Невозможно выполнить запрос. Идентификатор тип техники пустой.")
            : storage.Put("v.kind_id = @kind_id", "@kind_id", _kindId);
    }
}