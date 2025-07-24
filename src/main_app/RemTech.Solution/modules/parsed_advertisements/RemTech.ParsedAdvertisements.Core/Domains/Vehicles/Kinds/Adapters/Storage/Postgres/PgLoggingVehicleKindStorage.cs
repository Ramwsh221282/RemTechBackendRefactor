using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports.Storage.Postgres;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Adapters.Storage.Postgres;

public sealed class PgLoggingVehicleKindStorage(ICustomLogger logger, IPgVehicleKindsStorage storage)
    : IPgVehicleKindsStorage
{
    public async Task<VehicleKind> Read(VehicleKind kind, CancellationToken ct = default)
    {
        try
        {
            VehicleKind fromDb = await storage.Read(kind, ct);
            string name = fromDb.Identify().ReadText();
            Guid id = fromDb.Identify().ReadId();
            logger.Info("Vehicle kind from db: ID - {0}. Name - {1}.", id, name);
            return fromDb;
        }
        catch(Exception ex)
        {
            logger.Error("Unable to fetch vehicle kind from db. Error: {0}.", ex.Message);
            throw;
        }
    }
}