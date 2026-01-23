using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehiclePhotosGallery
{
	public IReadOnlyList<VehiclePhoto> Photos { get; }

	private VehiclePhotosGallery(IReadOnlyList<VehiclePhoto> photos)
	{
		Photos = photos;
	}

	public static Result<VehiclePhotosGallery> Create(IReadOnlyList<VehiclePhoto> photos)
	{
		if (photos.Count == 0)
			return Error.Validation("Фотографии техники не могут отсутствовать.");
		return new VehiclePhotosGallery(photos);
	}
}
