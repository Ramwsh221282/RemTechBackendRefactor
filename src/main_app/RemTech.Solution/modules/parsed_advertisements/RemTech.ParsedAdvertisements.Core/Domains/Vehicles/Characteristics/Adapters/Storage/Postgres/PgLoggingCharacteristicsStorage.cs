using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;

public sealed class PgLoggingCharacteristicsStorage : IPgCharacteristicsStorage
{
    private readonly ICustomLogger _logger;
    private readonly IPgCharacteristicsStorage _storage;

    public PgLoggingCharacteristicsStorage(ICustomLogger logger, IPgCharacteristicsStorage storage)
    {
        _logger = logger;
        _storage = storage;
    }
    
    public async Task<ICharacteristic> Stored(UnstructuredCharacteristic unstructured, CancellationToken ct = default)
    {
        try
        {
            ICharacteristic ctx = await _storage.Stored(unstructured, ct);
            Guid id = ctx.Identify().ReadId();
            string name = ctx.Identify().ReadText();
            string measure = ctx.Measure().Read();
            _logger.Info("Stored characteristic: ID - {0}. Name - {1}. Measure - {2}.", id, name, measure);
            return ctx;
        }
        catch(Exception ex)
        {
            _logger.Info("Unable to store characteristic. Error - {0}.", ex.Message);
            throw;
        }
    }
}