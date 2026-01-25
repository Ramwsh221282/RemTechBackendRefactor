using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehiclePhotosGallery
{
	private VehiclePhotosGallery(IReadOnlyList<VehiclePhoto> photos)
	{
		Photos = photos;
	}

	public IReadOnlyList<VehiclePhoto> Photos { get; }

	public static Result<VehiclePhotosGallery> Create(IReadOnlyList<VehiclePhoto> photos)
	{
		return photos.Count == 0
			? (Result<VehiclePhotosGallery>)Error.Validation("Фотографии техники не могут отсутствовать.")
			: (Result<VehiclePhotosGallery>)new VehiclePhotosGallery(photos);
	}
}
