using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.CategoryContext.ValueObjects;

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

    public static Status<CategoryId> Create(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            return Error.Validation("Идентификатор категории техники был пустым.");
        return new CategoryId(categoryId);
    }
}
