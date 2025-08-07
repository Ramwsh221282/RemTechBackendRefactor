namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;

public sealed class ExceptionLoggingVehicle : IAvitoVehicle
{
    private readonly string _itemUrl;
    private readonly IAvitoVehicle _origin;
    private readonly Serilog.ILogger _log;

    public ExceptionLoggingVehicle(string itemUrl, Serilog.ILogger log, IAvitoVehicle origin)
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
        catch (Exception ex)
        {
            _log.Error("Exception at scraping vehicle with url: {0}.", _itemUrl);
            _log.Error("Exception message: {0}.", ex.Message);
            return new AvitoVehicleEnvelope();
        }
    }
}
