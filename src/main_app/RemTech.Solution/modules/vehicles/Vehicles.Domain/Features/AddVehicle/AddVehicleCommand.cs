using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Domain.Features.AddVehicle;

public sealed record AddVehicleCommand(
    AddVehicleCreatorCommandPayload Creator,
    IEnumerable<AddVehicleVehiclesCommandPayload> Vehicles
) : ICommand;