namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Ports.Storage.Postgres;

public interface IPgVehicleModelsStorage
{
    Task<VehicleModel> Get(VehicleModel model, CancellationToken ct = default);
}