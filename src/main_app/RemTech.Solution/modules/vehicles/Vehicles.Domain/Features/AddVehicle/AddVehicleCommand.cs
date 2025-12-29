using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Domain.Features.AddVehicle;

public sealed record AddVehicleCommand(
    Guid CreatorId,
    string CreatorDomain,
    string CreatorType,
    Guid Id,
    string Title,
    string Url,
    long Price,
    bool IsNds,
    string Address,
    string[] Photos,
    AddVehicleCommandCharacteristics[] Characteristics
) : ICommand;