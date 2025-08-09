namespace Parsing.RabbitMq.PublishVehicle;

public sealed record VehicleBody(
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
