using FluentValidation;
using ParsedAdvertisements.Domain.BrandContext.ValueObjects;
using ParsedAdvertisements.Domain.CategoryContext.ValueObjects;
using ParsedAdvertisements.Domain.ModelContext.ValueObjects;
using ParsedAdvertisements.Domain.RegionContext.ValueObjects;
using ParsedAdvertisements.Domain.VehicleContext.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace ParsedAdvertisements.Domain.Common.UseCases.AddVehicle;

public sealed class AddVehicleCommandValidator : AbstractValidator<AddVehicleCommand>
{
    public AddVehicleCommandValidator()
    {
        RuleFor(x => x.CategoryName)
            .MustBeValid(x => CategoryName.Create(x));
        RuleFor(x => x.BrandName)
            .MustBeValid(x => BrandName.Create(x));
        RuleFor(x => x.ModelName)
            .MustBeValid(x => ModelName.Create(x));
        RuleFor(x => x.Photos)
            .MustBeValid(x => UniquePhotoColleciton.Create(x));
        RuleFor(x => x.Characteristics.Select(c => c.Name))
            .MustBeValid(x => UniqueCharacteristicNamesCollection.Create(x));
        RuleFor(x => new { x.IsNds, x.Price })
            .MustBeValid(x => VehiclePriceSpecification.Create(x.Price));
        RuleFor(x => x.Locations)
            .MustBeValid(x => UniqueVehicleLocationPartsCollection.Create(x));
    }
}