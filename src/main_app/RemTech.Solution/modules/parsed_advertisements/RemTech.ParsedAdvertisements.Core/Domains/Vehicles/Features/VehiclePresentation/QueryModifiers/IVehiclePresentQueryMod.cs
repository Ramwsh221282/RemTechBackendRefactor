namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.QueryModifiers;

public interface IVehiclePresentQueryMod
{
    VehiclePresentQueryStorage Modified(VehiclePresentQueryStorage storage);
}