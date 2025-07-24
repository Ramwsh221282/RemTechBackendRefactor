using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports.Storage;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;

public sealed class PgLoggingVehicleBrandsStorage(
    ICustomLogger logger,
    IPgVehicleBrandsStorage storage)
    : IPgVehicleBrandsStorage
{
    public async Task<VehicleBrand> Get(VehicleBrand brand, CancellationToken ct = default)
    {
        try
        {
            VehicleBrand stored = await storage.Get(brand, ct);
            Guid id = stored.Identify().ReadId();
            string name = stored.Identify().ReadText();
            logger.Info("Vehicle brand: ID - {0}. Name - {1} stored.", id, name);
            return stored;
        }
        catch(Exception ex)
        {
            logger.Error("Unable to process vehicle brand into storage. Error: {0}.", ex.Message);
            throw;
        }
    }
}