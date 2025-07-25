using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class LoggingCharacteristics : IKeyValuedCharacteristicsSource
{
    private readonly ICustomLogger _log;
    private readonly IKeyValuedCharacteristicsSource _origin;

    public LoggingCharacteristics(ICustomLogger log, IKeyValuedCharacteristicsSource origin)
    {
        _log = log;
        _origin = origin;
    }
    
    public async Task<CharacteristicsDictionary> Read()
    {
        CharacteristicsDictionary ctxes = await _origin.Read();
        if (ctxes)
        {
            foreach (var ctx in ctxes.Read())
                _log.Info("Characteristic. {0}:{1}.", ctx.Name(), ctx.Value());
        }
        else
        {
            _log.Warn("Unable to read characteristics.");
        }
        
        return ctxes;
    }
}