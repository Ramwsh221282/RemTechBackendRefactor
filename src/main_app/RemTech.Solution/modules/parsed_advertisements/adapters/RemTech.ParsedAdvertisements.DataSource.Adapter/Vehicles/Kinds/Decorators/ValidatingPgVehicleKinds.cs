using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds.Decorators;

public sealed class ValidatingPgVehicleKinds(IAsyncVehicleKinds origin) : IAsyncVehicleKinds
{
    public Task<Status<IVehicleKind>> Add(IVehicleKind kind, CancellationToken ct = default)
    {
        if (!kind.Identify().ReadId())
            return new ValidationError<IVehicleKind>("Некорректный ID типа техники.");
        return !kind.Identify().ReadText()
            ? new ValidationError<IVehicleKind>("Некорректное название типа технкии.")
            : origin.Add(kind, ct);
    }

    public Task<MaybeBag<IVehicleKind>> Find(
        VehicleKindIdentity identity,
        CancellationToken ct = default
    ) =>
        !identity.ReadText() && !identity.ReadId()
            ? Task.FromResult(new MaybeBag<IVehicleKind>())
            : origin.Find(identity, ct);

    public void Dispose() => origin.Dispose();

    public async ValueTask DisposeAsync() => await origin.DisposeAsync();
}
