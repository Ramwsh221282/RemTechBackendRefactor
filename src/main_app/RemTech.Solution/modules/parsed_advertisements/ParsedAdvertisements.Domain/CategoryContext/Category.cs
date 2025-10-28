using ParsedAdvertisements.Domain.CategoryContext.ValueObjects;

namespace ParsedAdvertisements.Domain.CategoryContext;

public sealed class Category
{
    public CategoryId Id { get; }
    public CategoryName Name { get; }

    public Category(CategoryName name, CategoryId? id = null)
    {
        Name = name;
        Id = id ?? new CategoryId();
    }
}
