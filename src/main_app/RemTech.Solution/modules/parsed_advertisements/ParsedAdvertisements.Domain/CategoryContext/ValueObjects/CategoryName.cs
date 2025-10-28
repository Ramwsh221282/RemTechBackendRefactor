using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.CategoryContext.ValueObjects;

public sealed record CategoryName
{
    public const int MaxLength = 100;
    public string Name { get; }

    private CategoryName(string name)
    {
        Name = name;
    }

    public static Status<CategoryName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Название категории было пустым.");

        if (name.Length > MaxLength)
            return Error.Validation($"Название категории превышает длину: {MaxLength} символов.");

        return new CategoryName(name);
    }
}
