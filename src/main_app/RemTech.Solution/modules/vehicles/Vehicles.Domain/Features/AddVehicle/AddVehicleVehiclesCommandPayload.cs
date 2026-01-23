namespace Vehicles.Domain.Features.AddVehicle;

public sealed record AddVehicleVehiclesCommandPayload(
    Guid Id,
    string Title,
    string Url,
    long Price,
    bool IsNds,
    string Address,
    string[] Photos,
    AddVehicleCommandCharacteristics[] Characteristics
);
