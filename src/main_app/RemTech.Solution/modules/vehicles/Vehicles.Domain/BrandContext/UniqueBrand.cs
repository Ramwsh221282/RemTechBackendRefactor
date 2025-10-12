using RemTech.Result.Pattern;
using Vehicles.Domain.BrandContext.Errors;
using Vehicles.Domain.BrandContext.ValueObjects;

namespace Vehicles.Domain.BrandContext;

public sealed class UniqueBrand
{
    private readonly BrandName? _name;

    public UniqueBrand(BrandName? name) => _name = name;

    public Result<Brand> ApproveUniqueness(Brand brand) =>
        brand.Name == _name ? new BrandNameNotUniqueError(_name) : brand;
}
