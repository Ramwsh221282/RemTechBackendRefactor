namespace RemTech.Vehicles.Module.Types.Models.Storage;

internal sealed class VarietVehicleModelsStorage : IVehicleModelsStorage
{
    private readonly Queue<IVehicleModelsStorage> _storages = [];

    public VarietVehicleModelsStorage With(IVehicleModelsStorage storage)
    {
        _storages.Enqueue(storage);
        return this;
    }

    public async Task<VehicleModel> Store(VehicleModel vehicleModel)
    {
        while (_storages.Count > 0)
        {
            IVehicleModelsStorage storage = _storages.Dequeue();
            try
            {
                return await storage.Store(vehicleModel);
            }
            catch (UnableToStoreVehicleModelException ex) { }
        }

        throw new UnableToStoreVehicleModelException(
            "Unable to save vehicle model",
            vehicleModel.NameString()
        );
    }
}
