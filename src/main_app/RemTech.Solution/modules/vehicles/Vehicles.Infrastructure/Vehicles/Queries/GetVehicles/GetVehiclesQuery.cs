using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

/// <summary>
/// Запрос на получение транспортных средств.
/// </summary>
/// <param name="parameters">Параметры запроса для получения транспортных средств.</param>
public sealed class GetVehiclesQuery(GetVehiclesQueryParameters parameters) : IQuery
{
	/// <summary>
	/// Параметры запроса для получения транспортных средств.
	/// </summary>
	public GetVehiclesQueryParameters Parameters { get; } = parameters;

	/// <summary>
	/// Преобразует параметры запроса в строковое представление.
	/// </summary>
	/// <returns>Строковое представление параметров запроса.</returns>
	public override string ToString() => Parameters.ToString();
}
