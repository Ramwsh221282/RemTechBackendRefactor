using Parsing.SDK.Logging;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicle;

public sealed class ExceptionLoggingVehicle : IAvitoVehicle
{
    private readonly string _itemUrl;
    private readonly IAvitoVehicle _origin;
    private readonly IParsingLog _log;

    public ExceptionLoggingVehicle(string itemUrl, IParsingLog log,  IAvitoVehicle origin)
    {
        _itemUrl = itemUrl;
        _log = log;
        _origin = origin;
    }
    
    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        try
        {
            AvitoVehicleEnvelope result = await _origin.VehicleSource();
            return result;
        }
        catch(Exception ex)
        {
            _log.Error("Exception at scraping vehicle with url: {0}.", _itemUrl);
            _log.Error("Exception message: {0}.", ex.Message);
            return new AvitoVehicleEnvelope();
        }
    }
}