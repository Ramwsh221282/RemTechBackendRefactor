using Parsing.SDK.Logging;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class LoggingCharacteristics : IKeyValuedCharacteristicsSource
{
    private readonly IParsingLog _log;
    private readonly IKeyValuedCharacteristicsSource _origin;

    public LoggingCharacteristics(IParsingLog log, IKeyValuedCharacteristicsSource origin)
    {
        _log = log;
        _origin = origin;
    }
    
    public async Task<KeyValueVehicleCharacteristics> Read()
    {
        KeyValueVehicleCharacteristics ctxes = await _origin.Read();
        if (ctxes)
        {
            foreach (var ctx in ctxes.Read())
                _log.Info("Characteristic. {0}:{1}.", ctx.key, ctx.value);
        }
        else
        {
            _log.Warning("Unable to read characteristics.");
        }
        
        return ctxes;
    }
}