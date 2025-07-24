using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Ports.Storage.Postgres;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Adapters.Storage.Postgres;

public sealed class PgLoggingVehicleModelsStorage : IPgVehicleModelsStorage
{
    private readonly ICustomLogger _logger;
    private readonly IPgVehicleModelsStorage _origin;

    public PgLoggingVehicleModelsStorage(ICustomLogger logger, IPgVehicleModelsStorage origin)
    {
        _logger = logger;
        _origin = origin;
    }
    
    public async Task<VehicleModel> Get(VehicleModel model, CancellationToken ct = default)
    {
        try
        {
            VehicleModel fromDb = await _origin.Get(model);
            _logger.Info(fromDb.LogString());
            return fromDb;
        }
        catch(Exception ex)
        {
            _logger.Error("Unable to store vehicle model. {0}. Error: {1}.", model.LogString(), ex.Message);
            throw;
        }
    }
}