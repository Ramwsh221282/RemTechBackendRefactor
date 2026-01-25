using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Models;

public sealed record ModelName
{
    private const int MaxLength = 128;
    public string Value { get; }

    private ModelName(string value)
    {
        Value = value;
    }

    public static Result<ModelName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Имя модели не может быть пустым.");
        return value.Length > MaxLength ? (Result<ModelName>)Error.Validation($"Имя модели не может быть больше {MaxLength} символов.") : (Result<ModelName>)new ModelName(value);
    }
}
