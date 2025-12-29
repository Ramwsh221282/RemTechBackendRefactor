using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Brands;

public sealed record BrandName
{
    private const int MaxLength = 128;
    public string Name { get; }

    private BrandName(string name)
    {
        Name = name;
    }
    
    public static Result<BrandName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Название бренда не может быть пустым.");
        if (name.Length > MaxLength)
            return Error.Validation($"Название бренда превышает {MaxLength} символов.");
        return new BrandName(name);
    }
}