using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

public sealed class ContainedItemType
{
    public string Value { get; }
    private ContainedItemType(string value) => Value = value;
    public static Result<ContainedItemType> Create(string value)
    {
        if (value.Length > 128) return Error.Validation("Тип элемента слишком длинный.");
        return Result.Success(new ContainedItemType(value));
    }
}