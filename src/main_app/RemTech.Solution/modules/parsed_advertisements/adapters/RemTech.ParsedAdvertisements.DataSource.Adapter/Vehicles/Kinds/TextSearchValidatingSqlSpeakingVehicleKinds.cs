using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds;

public sealed class TextSearchValidatingSqlSpeakingVehicleKinds(ISqlSpeakingKinds origin)
    : ISqlSpeakingKinds
{
    public Task<Status<IVehicleKind>> Add(IVehicleKind kind, CancellationToken ct = default)
    {
        string name = kind.Identify().ReadText();
        return string.IsNullOrWhiteSpace(name)
            ? new ValidationError<IVehicleKind>("Пустое название типа техники.")
            : origin.Add(kind, ct);
    }

    public Task<MaybeBag<IVehicleKind>> Find(
        VehicleKindIdentity identity,
        CancellationToken ct = default
    )
    {
        string name = identity.ReadText();
        return string.IsNullOrWhiteSpace(name)
            ? Task.FromResult(new MaybeBag<IVehicleKind>())
            : origin.Find(identity, ct);
    }

    public void Dispose() => origin.Dispose();

    public ValueTask DisposeAsync() => origin.DisposeAsync();
}
