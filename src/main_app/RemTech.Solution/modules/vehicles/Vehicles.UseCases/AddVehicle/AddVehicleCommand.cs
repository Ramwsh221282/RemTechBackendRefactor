using FluentValidation;
using RemTech.UseCases.Shared.Cqrs;
using RemTech.UseCases.Shared.Validations;
using Vehicles.Domain.BrandContext.ValueObjects;
using Vehicles.Domain.CategoryContext.ValueObjects;
using Vehicles.Domain.LocationContext.ValueObjects;
using Vehicles.Domain.ModelContext.ValueObjects;
using Vehicles.Domain.VehicleContext;
using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.UseCases.AddVehicle;

public sealed record AddVehicleCommand(
    string CategoryName,
    string BrandName,
    string ModelName,
    IEnumerable<string> LocationParts,
    string Description,
    AddVehicleCommandPriceInfo Price,
    IEnumerable<AddVehicleCommandCharacteristic> Characteristics,
    IEnumerable<string> PhotoPaths
) : ICommand<Vehicle>;

public sealed class AddVehicleCommandValidator : AbstractValidator<AddVehicleCommand>
{
    public AddVehicleCommandValidator()
    {
        RuleFor(x => x.CategoryName).MustBeValid(CategoryName.Create);
        RuleFor(x => x.BrandName).MustBeValid(BrandName.Create);
        RuleFor(x => x.ModelName).MustBeValid(VehicleModelName.Create);
        RuleFor(x => x.LocationParts).MustBeValid(LocationAddress.Create);
        RuleFor(x => x.Description).MustBeValid(VehicleDescription.Create);

        RuleFor(x => new { x.Price.Value, x.Price.IsNds })
            .MustBeValid(arg => VehiclePrice.Create(arg.Value, arg.IsNds));

        RuleFor(x => x.Characteristics)
            .AllMustBeValid(c => VehicleCharacteristic.Create(c.Name, c.Value));

        RuleFor(x => x.Characteristics)
            .MustBeValid(en =>
                VehicleCharacteristicsCollection.Create(
                    en.Select(ctx => VehicleCharacteristic.Create(ctx.Name, ctx.Value).Value)
                )
            );

        RuleFor(x => x.PhotoPaths).AllMustBeValid(VehiclePhoto.Create);
        RuleFor(x => x.PhotoPaths).MustBeValid(VehiclePhotosCollection.Create);
    }
}
