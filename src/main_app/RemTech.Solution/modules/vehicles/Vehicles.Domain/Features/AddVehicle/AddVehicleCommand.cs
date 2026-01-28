using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Domain.Features.AddVehicle;

/// <summary>
/// Команда добавления транспортного средства.
/// </summary>
/// <param name="Creator">Создатель транспортного средства.</param>
/// <param name="Vehicles">Список транспортных средств для добавления.</param>
public sealed record AddVehicleCommand(
	AddVehicleCreatorCommandPayload Creator,
	IEnumerable<AddVehicleVehiclesCommandPayload> Vehicles
) : ICommand;
