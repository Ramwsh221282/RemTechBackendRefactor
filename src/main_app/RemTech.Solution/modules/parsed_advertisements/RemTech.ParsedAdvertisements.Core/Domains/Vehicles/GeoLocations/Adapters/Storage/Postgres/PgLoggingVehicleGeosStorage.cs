using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports.Storage.Postgres;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Adapters.Storage.Postgres;

public sealed class PgLoggingVehicleGeosStorage(
    ICustomLogger logger,
    IPgVehicleGeosStorage origin)
    : IPgVehicleGeosStorage
{
    public async Task<GeoLocation> Get(GeoLocation location, CancellationToken ct = default)
    {
        try
        {
            GeoLocation stored = await origin.Get(location, ct);
            string name = stored.Identify().ReadText();
            Guid id = stored.Identify().ReadId();
            logger.Info("Stored vehicle location. ID - {0}. Name - {1}.", id, name);
            return stored;
        }
        catch(Exception ex)
        {
            logger.Error("Unable to store vehicle location. Error: {0}.", ex.Message);
            throw;
        }
    }
}