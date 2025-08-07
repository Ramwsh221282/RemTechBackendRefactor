namespace RemTech.Vehicles.Module.Features.SinkVehicles;

internal sealed record VehicleSinkMessage(
    ParserBody Parser,
    ParserLinkBody Link,
    VehicleBody Vehicle
);
