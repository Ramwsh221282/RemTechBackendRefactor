using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Transport;

public interface ISqlSpeakingVehicles : IDisposable, IAsyncDisposable
{
    Task<Status<IVehicle>> SqlAdded(IVehicle vehicle, IVehicles vehicles);
}
