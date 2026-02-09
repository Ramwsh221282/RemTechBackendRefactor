namespace Vehicles.Domain.Features.AddVehicle;

/// <summary>
/// Транспортное средство для команды добавления транспортного средства.
/// </summary>
/// <param name="Id">Идентификатор транспортного средства.</param>
/// <param name="Title">Название транспортного средства.</param>
/// <param name="Url">URL транспортного средства.</param>
/// <param name="Price">Цена транспортного средства.</param>
/// <param name="IsNds">Флаг, указывающий, включен ли НДС.</param>
/// <param name="Address">Адрес транспортного средства.</param>
/// <param name="Photos">Фотографии транспортного средства.</param>
/// <param name="Characteristics">Характеристики транспортного средства.</param>
public sealed record AddVehicleVehiclesCommandPayload(
	Guid Id,
	string Title,
	string Url,
	long Price,
	bool IsNds,
	string Address,
	IReadOnlyList<string> Photos,
	IReadOnlyList<AddVehicleCommandCharacteristics> Characteristics
);
