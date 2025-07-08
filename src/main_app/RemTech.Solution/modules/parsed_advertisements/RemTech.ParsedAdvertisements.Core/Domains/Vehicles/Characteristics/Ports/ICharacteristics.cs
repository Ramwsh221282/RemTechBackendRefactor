namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports;

public interface ICharacteristics
{
    Task<ParsedVehicleCharacteristic[]> AddOrGet(
        ParsedVehicleCharacteristic[] characteristics,
        CancellationToken ct = default
    );
}
