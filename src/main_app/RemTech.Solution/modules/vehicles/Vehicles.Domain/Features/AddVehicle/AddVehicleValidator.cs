using FluentValidation;
using RemTech.SharedKernel.Core.Handlers;
using Vehicles.Domain.Brands;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Models;
using Vehicles.Domain.Vehicles;

namespace Vehicles.Domain.Features.AddVehicle;

public sealed class AddVehicleValidator : AbstractValidator<AddVehicleCommand>
{
    public AddVehicleValidator()
    {
        RuleFor(x => x.CreatorId).NotEmpty();
        RuleFor(x => x.CreatorDomain).NotEmpty();
        RuleFor(x => x.CreatorType).NotEmpty();
        RuleFor(x => x.Id).MustBeValid(VehicleId.Create);
        RuleFor(x => x.Title).MustBeValid(VehicleTextInformation.Create);
        RuleFor(x => x.Url).MustBeValid(VehicleSource.Create);
        RuleFor(x => new { x.Price, x.IsNds }).MustBeValid(o => VehiclePriceInformation.Create(o.Price, o.IsNds));
        RuleFor(x => x.Address).MustBeValid(LocationName.Create);
        RuleFor(x => x.Title).MustBeValid(BrandName.Create);
        RuleFor(x => x.Title).MustBeValid(ModelName.Create);
        RuleFor(x => x.Title).MustBeValid(CategoryName.Create);
    }
}