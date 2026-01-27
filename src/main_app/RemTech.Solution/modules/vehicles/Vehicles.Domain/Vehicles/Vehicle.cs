using Vehicles.Domain.Brands;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Models;

namespace Vehicles.Domain.Vehicles;

/// <summary>
/// Транспортное средство.
/// </summary>
/// <param name="id">Идентификатор транспортного средства.</param>
/// <param name="brand">Идентификатор бренда транспортного средства.</param>
/// <param name="category">Идентификатор категории транспортного средства.</param>
/// <param name="location">Идентификатор местоположения транспортного средства.</param>
/// <param name="model">Идентификатор модели транспортного средства.</param>
/// <param name="source">Источник информации о транспортном средстве.</param>
/// <param name="price">Информация о цене транспортного средства.</param>
/// <param name="text">Текстовая информация о транспортном средстве.</param>
/// <param name="photos">Галерея фотографий транспортного средства.</param>
public sealed class Vehicle(
	VehicleId id,
	BrandId brand,
	CategoryId category,
	LocationId location,
	ModelId model,
	VehicleSource source,
	VehiclePriceInformation price,
	VehicleTextInformation text,
	VehiclePhotosGallery photos
)
{
	/// <summary>
	/// Создаёт транспортное средство.
	/// </summary>
	/// <param name="id">Идентификатор транспортного средства.</param>
	/// <param name="brand">Идентификатор бренда транспортного средства.</param>
	/// <param name="category">Идентификатор категории транспортного средства.</param>
	/// <param name="location">Идентификатор местоположения транспортного средства.</param>
	/// <param name="model">Идентификатор модели транспортного средства.</param>
	/// <param name="source">Источник информации о транспортном средстве.</param>
	/// <param name="price">Информация о цене транспортного средства.</param>
	/// <param name="text">Текстовая информация о транспортном средстве.</param>
	/// <param name="photos">Галерея фотографий транспортного средства.</param>
	public Vehicle(
		VehicleId id,
		Brand brand,
		Category category,
		Location location,
		Model model,
		VehicleSource source,
		VehiclePriceInformation price,
		VehicleTextInformation text,
		VehiclePhotosGallery photos
	)
		: this(id, brand.Id, category.Id, location.Id, model.Id, source, price, text, photos) { }

	/// <summary>
	/// Идентификатор транспортного средства.
	/// </summary>
	public VehicleId Id { get; } = id;

	/// <summary>
	/// Идентификатор бренда транспортного средства.
	/// </summary>
	public BrandId BrandId { get; } = brand;

	/// <summary>
	///     Идентификатор категории транспортного средства.
	/// </summary>
	public CategoryId CategoryId { get; } = category;

	/// <summary>
	///   Идентификатор местоположения транспортного средства.
	/// </summary>
	public LocationId LocationId { get; } = location;

	/// <summary>
	///  Идентификатор модели транспортного средства.
	/// </summary>
	public ModelId ModelId { get; } = model;

	/// <summary>
	/// Источник информации о транспортном средстве.
	/// </summary>
	public VehicleSource Source { get; } = source;

	/// <summary>
	/// Информация о цене транспортного средства.
	/// </summary>
	public VehiclePriceInformation Price { get; } = price;

	/// <summary>
	/// Текстовая информация о транспортном средстве.
	/// </summary>
	public VehicleTextInformation Text { get; } = text;

	/// <summary>
	/// Галерея фотографий транспортного средства.
	/// </summary>
	public VehiclePhotosGallery Photos { get; } = photos;

	/// <summary>
	/// Характеристики транспортного средства.
	/// </summary>
	public IReadOnlyList<VehicleCharacteristic> Characteristics { get; private set; } = [];

	/// <summary>
	/// Добавляет характеристики транспортного средства.
	/// </summary>
	/// <param name="characteristics">Коллекция характеристик для добавления.</param>
	public void AddCharacteristics(IEnumerable<VehicleCharacteristicToAdd> characteristics)
	{
		VehicleCharacteristicByNameComparer comparer = new VehicleCharacteristicByNameComparer();
		HashSet<VehicleCharacteristic> result = new(comparer);
		foreach (VehicleCharacteristicToAdd characteristic in characteristics)
			result.Add(new VehicleCharacteristic(this, characteristic.Characteristic, characteristic.Value));
		Characteristics = [.. result];
	}
}
