namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public interface IAvitoDescriptionParts
{
    Task<string> Read();
}
