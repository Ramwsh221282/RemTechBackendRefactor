using Vehicles.Domain.Brands;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Models;

namespace Vehicles.Domain.Vehicles;

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
