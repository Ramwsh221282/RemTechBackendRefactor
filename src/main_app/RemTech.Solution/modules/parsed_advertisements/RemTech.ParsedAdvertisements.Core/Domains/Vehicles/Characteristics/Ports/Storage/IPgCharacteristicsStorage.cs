namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;

public interface IPgCharacteristicsStorage
{
    Task<Characteristic> Stored(Characteristic unstructured, CancellationToken ct = default);
}