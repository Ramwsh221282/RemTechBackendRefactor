using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Categories;

public sealed record CategoryName
{
    private const int MaxLength = 128;
    public string Value { get; }

    private CategoryName(string value)
    {
        Value = value;
    }

    public static Result<CategoryName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Имя категории не может быть пустым.");
        return value.Length > MaxLength
            ? (Result<CategoryName>)Error.Validation($"Имя категории не может быть больше {MaxLength} символов.")
            : (Result<CategoryName>)new CategoryName(value);
    }
}
