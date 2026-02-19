namespace Vehicles.Domain.Vehicles;

/// <summary>
/// Галерея фотографий транспортного средства.
/// </summary>
public sealed record VehiclePhotosGallery
{
	private VehiclePhotosGallery(IReadOnlyList<VehiclePhoto> photos)
	{
		Photos = photos;
	}

	/// <summary>
	/// Фотографии транспортного средства.
	/// </summary>
	public IReadOnlyList<VehiclePhoto> Photos { get; }

	/// <summary>
	/// Создаёт галерею фотографий транспортного средства.
	/// </summary>
	/// <param name="photos">Фотографии транспортного средства.</param>
	/// <returns>Результат создания галереи фотографий транспортного средства.</returns>
	public static VehiclePhotosGallery Create(IReadOnlyList<VehiclePhoto> photos)
	{		
		return new VehiclePhotosGallery(photos);
	}
}
