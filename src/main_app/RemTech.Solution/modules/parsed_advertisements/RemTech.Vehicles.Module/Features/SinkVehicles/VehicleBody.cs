namespace RemTech.Vehicles.Module.Features.SinkVehicles;

internal sealed record VehicleBody(
    string Id,
    string Kind,
    string Brand,
    string Model,
    long Price,
    bool IsNds,
    string Geo,
    string SourceUrl,
    string Description,
    IEnumerable<VehicleBodyCharacteristic> Characteristics,
    IEnumerable<VehicleBodyPhoto> Photos
);
