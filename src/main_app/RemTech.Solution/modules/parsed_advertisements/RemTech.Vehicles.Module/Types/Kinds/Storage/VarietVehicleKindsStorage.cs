namespace RemTech.Vehicles.Module.Types.Kinds.Storage;

internal sealed class VarietVehicleKindsStorage : IVehicleKindsStorage
{
    private readonly Queue<IVehicleKindsStorage> _storages = [];

    public VarietVehicleKindsStorage With(IVehicleKindsStorage storage)
    {
        _storages.Enqueue(storage);
        return this;
    }

    public async Task<VehicleKind> Store(VehicleKind kind)
    {
        while (_storages.Count > 0)
        {
            IVehicleKindsStorage storage = _storages.Dequeue();
            try
            {
                return await storage.Store(kind);
            }
            catch (UnableToStoreVehicleKindException) { }
        }

        throw new UnableToStoreVehicleKindException("Unable to save vehicle kind");
    }
}
