using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Locations;

public readonly record struct LocationId
{
	public LocationId()
	{
		Id = Guid.NewGuid();
	}

	private LocationId(Guid id)
	{
		Id = id;
	}

	public Guid Id { get; }

	public static Result<LocationId> Create(Guid id)
	{
		return id == Guid.Empty
			? (Result<LocationId>)Error.Validation("Идентификатор локации не может быть пустым.")
			: (Result<LocationId>)new LocationId(id);
	}
}
