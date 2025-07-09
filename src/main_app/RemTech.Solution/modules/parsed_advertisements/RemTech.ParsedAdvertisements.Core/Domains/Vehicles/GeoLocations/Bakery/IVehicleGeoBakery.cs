using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery;

public interface IVehicleGeoBakery
{
    public Status<IGeoLocation> Baked(IVehicleGeoReceipt receipt);
}
