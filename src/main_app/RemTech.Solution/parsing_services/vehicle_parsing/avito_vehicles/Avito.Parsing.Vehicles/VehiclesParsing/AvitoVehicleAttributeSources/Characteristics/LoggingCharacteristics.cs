﻿using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using Serilog;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class LoggingCharacteristics : IKeyValuedCharacteristicsSource
{
    private readonly ILogger _log;
    private readonly IKeyValuedCharacteristicsSource _origin;

    public LoggingCharacteristics(ILogger log, IKeyValuedCharacteristicsSource origin)
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
                _log.Information("Characteristic. {0}:{1}.", ctx.Name(), ctx.Value());
        }
        else
        {
            _log.Warning("Unable to read characteristics.");
        }
        
        return ctxes;
    }
}