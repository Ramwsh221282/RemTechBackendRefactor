using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Locations;

public readonly record struct LocationId
{
    public Guid Id { get; }

    public LocationId()
    {
        Id = Guid.NewGuid();
    }
    
    private LocationId(Guid id)
    {
        Id = id;
    }

    public static Result<LocationId> Create(Guid id)
    {
        if (id == Guid.Empty)
            return Error.Validation("Идентификатор локации не может быть пустым.");
        return new LocationId(id);
    }
}