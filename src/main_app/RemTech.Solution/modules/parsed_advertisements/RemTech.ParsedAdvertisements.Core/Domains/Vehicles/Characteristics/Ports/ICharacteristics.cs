namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports;

public interface ICharacteristics
{
    Task<Characteristic[]> Similar(
        Characteristic[] characteristics,
        CancellationToken ct = default
    );
}
