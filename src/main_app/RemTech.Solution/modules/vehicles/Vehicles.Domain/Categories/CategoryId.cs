using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Categories;

public readonly record struct CategoryId
{
    public Guid Id { get; }

    public CategoryId()
    {
        Id = Guid.NewGuid();
    }
    
    private CategoryId(Guid id)
    {
        Id = id;
    }

    public static Result<CategoryId> Create(Guid id)
    {
        if (id == Guid.Empty)
            return Error.Validation("Идентификатор категории не может быть пустым.");
        return new CategoryId(id);
    }
}