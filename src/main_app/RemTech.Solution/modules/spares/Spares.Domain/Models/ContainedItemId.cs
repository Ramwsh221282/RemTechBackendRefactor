using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public readonly record struct ContainedItemId
{
    public Guid Value { get; }
    public ContainedItemId() => Value = Guid.NewGuid();
    private ContainedItemId(Guid value) => Value = value;
    
    public static Result<ContainedItemId> Create(Guid value) =>
        value == Guid.Empty 
            ? Error.Validation("Идентификатор запчасти не может быть пустым.") 
            : Result.Success(new ContainedItemId(value));
}