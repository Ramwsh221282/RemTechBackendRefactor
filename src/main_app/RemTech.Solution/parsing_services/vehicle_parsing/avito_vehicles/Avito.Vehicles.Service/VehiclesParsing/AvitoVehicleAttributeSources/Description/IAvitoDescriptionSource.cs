namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Description;

public interface IAvitoDescriptionSource
{
    Task<string> Read();
}
