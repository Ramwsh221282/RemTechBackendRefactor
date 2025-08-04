namespace RemTech.Vehicles.Module.Types.Brands.Storage;

internal sealed class VarietVehicleBrandsStorage : IVehicleBrandsStorage
{
    private readonly Queue<IVehicleBrandsStorage> _storages = [];

    public VarietVehicleBrandsStorage With(IVehicleBrandsStorage storage)
    {
        _storages.Enqueue(storage);
        return this;
    }

    public async Task<VehicleBrand> Store(VehicleBrand brand)
    {
        while (_storages.Count > 0)
        {
            IVehicleBrandsStorage storage = _storages.Dequeue();
            try
            {
                return await storage.Store(brand);
            }
            catch (UnableToStoreBrandException ex) { }
        }

        throw new ApplicationException("Unable to store vehicle brand.");
    }
}
