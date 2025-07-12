using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Characteristics;

public interface IAsyncCharacteristics : IDisposable, IAsyncDisposable
{
    Task<Status<ICharacteristic>> Add(
        ICharacteristic characteristic,
        CancellationToken ct = default
    );

    Task<MaybeBag<ICharacteristic>> Find(
        CharacteristicIdentity identity,
        CancellationToken ct = default
    );
}
