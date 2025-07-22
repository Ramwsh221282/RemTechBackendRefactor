namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicle;

public interface IAvitoVehicle
{
    Task<AvitoVehicleEnvelope> VehicleSource();
}