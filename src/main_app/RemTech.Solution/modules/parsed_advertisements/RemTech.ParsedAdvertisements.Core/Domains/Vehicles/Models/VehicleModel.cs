using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Adapters.Storage.Postgres;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;

public sealed class VehicleModel
{
    private readonly VehicleModelIdentity _identity;
    private readonly VehicleModelName _name;

    public VehicleModel(VehicleModelIdentity identity, VehicleModelName name)
    {
        _identity = identity;
        _name = name;
    }

    public VehicleModel()
    {
        _identity = new VehicleModelIdentity();
        _name = new VehicleModelName();
    }

    public BrandedVehicleModel Print(BrandedVehicleModel branded)
    {
        return new BrandedVehicleModel(branded, _identity);
    }

    public PgVehicleModelFromStoreCommand FromStoreCommand() =>
        new(_name);

    public PgVehicleModelToStoreCommand ToStoreCommand() =>
        new(_identity, _name);

    public string LogString()
    {
        return $"""
               ID: {(Guid)_identity}
               Name: {(string)_name}
               """;
    }
}