using FluentValidation;
using RemTech.UseCases.Shared.Validations;
using Vehicles.Domain.BrandContext.ValueObjects;

namespace Vehicles.UseCases.BrandContext.Commands.CreateBrand;

public sealed class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Name).MustBeValid(BrandName.Create);
    }
}
