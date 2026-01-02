namespace Vehicles.Domain.Features.AddVehicle;

public sealed record AddVehicleCreatorCommandPayload(
    Guid CreatorId,
    string CreatorDomain,
    string CreatorType
);