namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;

public sealed class EmptyAvitoVehicle : IAvitoVehicle
{
    public Task<AvitoVehicleEnvelope> VehicleSource()
    {
        return Task.FromResult(new AvitoVehicleEnvelope());
    }
}
