using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Characteristics.Decorators;

public sealed class PgValidatingCharacteristics(IAsyncCharacteristics origin)
    : IAsyncCharacteristics
{
    public Task<Status<ICharacteristic>> Add(
        ICharacteristic characteristic,
        CancellationToken ct = default
    )
    {
        if (!characteristic.Identify().ReadText())
            return new ValidationError<ICharacteristic>("Некорректное название характеристики.");
        if (!characteristic.Identify().ReadId())
            return new ValidationError<ICharacteristic>(
                "Некорректный идентификатор характеристики."
            );
        return origin.Add(characteristic, ct);
    }

    public Task<MaybeBag<ICharacteristic>> Find(
        CharacteristicIdentity identity,
        CancellationToken ct = default
    )
    {
        return !identity.ReadId() && !identity.ReadText()
            ? Task.FromResult(new MaybeBag<ICharacteristic>())
            : origin.Find(identity, ct);
    }

    public void Dispose()
    {
        origin.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await origin.DisposeAsync();
    }
}
