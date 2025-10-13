using RemTech.Result.Pattern;
using Vehicles.Domain.ModelContext.Errors;
using Vehicles.Domain.ModelContext.ValueObjects;

namespace Vehicles.Domain.ModelContext;

public sealed record UniqueVehicleModel
{
    private readonly VehicleModelName? _name;

    public UniqueVehicleModel(VehicleModelName? name) => _name = name;

    public Result<VehicleModel> Unique(VehicleModel model) =>
        _name == model.Name ? new NotUniqueVehicleModelError(_name) : model;
}
