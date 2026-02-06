using Vehicles.Domain.Brands;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Models;

namespace Vehicles.Domain.Vehicles;

/// <summary>
/// Фабрика для создания транспортных средств.
/// </summary>
/// <param name="brand">Бренд транспортного средства.</param>
/// <param name="category">Категория транспортного средства.</param>
/// <param name="location">Местоположение транспортного средства.</param>
/// <param name="model">Модель транспортного средства.</param>
/// <param name="Characteristics">Характеристики транспортного средства.</param>
public sealed class VehicleFactory(
	Brand brand,
	Category category,
	Location location,
	Model model,
	IEnumerable<VehicleCharacteristicToAdd> Characteristics
)
{
	private Brand Brand { get; } = brand;
	private Category Category { get; } = category;
	private Location Location { get; } = location;
	private Model Model { get; } = model;
	private IEnumerable<VehicleCharacteristicToAdd> Characteristics { get; } = Characteristics;

	/// <summary>
	/// Создаёт транспортное средство.
	/// </summary>
	/// <param name="id">Идентификатор транспортного средства.</param>
	/// <param name="source">Источник транспортного средства.</param>
	/// <param name="price">Информация о цене транспортного средства.</param>
	/// <param name="text">Текстовая информация о транспортном средстве.</param>
	/// <param name="photos">Галерея фотографий транспортного средства.</param>
	/// <returns>Созданное транспортное средство.</returns>
	public Vehicle Construct(
		VehicleId id,
		VehicleSource source,
		VehiclePriceInformation price,
		VehicleTextInformation text,
		VehiclePhotosGallery photos
	)
	{
		Vehicle vehicle = new(id, Brand.Id, Category.Id, Location.Id, Model.Id, source, price, text, photos);
		vehicle.AddCharacteristics(Characteristics);
		return vehicle;
	}
}
