namespace RemTech.Vehicles.Module.Types.Characteristics.Ports.Storage;

public interface IPgCharacteristicsStorage
{
    Task<Characteristic> Stored(Characteristic unstructured, CancellationToken ct = default);
}
