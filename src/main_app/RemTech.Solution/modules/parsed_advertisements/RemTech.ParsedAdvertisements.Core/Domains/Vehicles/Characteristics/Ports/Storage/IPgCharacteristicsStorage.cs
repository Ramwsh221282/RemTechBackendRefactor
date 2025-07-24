using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;

public interface IPgCharacteristicsStorage
{
    Task<ICharacteristic> Stored(UnstructuredCharacteristic unstructured, CancellationToken ct = default);
}