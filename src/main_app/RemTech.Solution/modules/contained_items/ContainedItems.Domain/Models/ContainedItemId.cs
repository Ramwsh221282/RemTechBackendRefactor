using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

public readonly record struct ContainedItemId
{
    public Guid Value { get; }
    public ContainedItemId() => Value = Guid.NewGuid();
    private ContainedItemId(Guid value) => Value = value;
    public static ContainedItemId New() => new();

    public static Result<ContainedItemId> Create(Guid value) =>
        value == Guid.Empty ?
            Error.Validation("Идентификатор сохраняемого элемента не может быть пустым.") : new ContainedItemId(value);
}