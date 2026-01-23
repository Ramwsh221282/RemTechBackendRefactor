using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

public sealed record ServiceItemId
{
    private ServiceItemId(string value) => Value = value;

    public static Result<ServiceItemId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Идентификатор сохраняемого элемента не может быть пустым.");
        return value.Length > MaxLength
            ? (Result<ServiceItemId>)Error.Validation($"Идентификатор сохраняемого элемента не может превышать {MaxLength} символов.")
            : (Result<ServiceItemId>)new ServiceItemId(value);
    }

    private const int MaxLength = 255;
    public string Value { get; }
}
