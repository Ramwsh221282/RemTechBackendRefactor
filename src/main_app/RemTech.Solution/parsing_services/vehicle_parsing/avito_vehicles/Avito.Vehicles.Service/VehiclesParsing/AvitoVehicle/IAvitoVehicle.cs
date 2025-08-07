namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;

public interface IAvitoVehicle
{
    Task<AvitoVehicleEnvelope> VehicleSource();
}
