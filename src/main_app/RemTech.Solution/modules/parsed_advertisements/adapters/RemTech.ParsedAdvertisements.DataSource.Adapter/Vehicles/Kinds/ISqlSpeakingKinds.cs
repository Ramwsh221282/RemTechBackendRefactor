using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds;

public interface ISqlSpeakingKinds : IDisposable, IAsyncDisposable
{
    Task<Status<IVehicleKind>> Add(IVehicleKind kind, CancellationToken ct = default);
    Task<MaybeBag<IVehicleKind>> Find(VehicleKindIdentity identity, CancellationToken ct = default);
}
