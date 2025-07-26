using RemTech.Core.Shared.Exceptions;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.QueryModifiers;

public sealed class ModeledVehiclePresentMod : IVehiclePresentQueryMod
{
    private readonly Guid _modelId;

    public ModeledVehiclePresentMod(Guid modelId) => _modelId = modelId;
    
    public VehiclePresentQueryStorage Modified(VehiclePresentQueryStorage storage)
    {
        return _modelId == Guid.Empty
            ? throw new OperationException("Невозможно выполнить запрос. Идентификатор модели пустой.")
            : storage.Put($"v.model_id = @model_id", "@model_id", _modelId);
    }
}