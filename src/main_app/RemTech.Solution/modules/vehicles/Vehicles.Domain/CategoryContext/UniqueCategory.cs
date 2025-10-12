using RemTech.Result.Pattern;
using Vehicles.Domain.CategoryContext.Errors;
using Vehicles.Domain.CategoryContext.ValueObjects;

namespace Vehicles.Domain.CategoryContext;

public sealed class UniqueCategory
{
    private readonly CategoryName? _name;

    public UniqueCategory(CategoryName? name) => _name = name;

    public Result<Category> ApproveUniqueness(Category category)
    {
        return _name == category.Name ? new CategoryNameNotUniqueError(_name) : category;
    }
}
