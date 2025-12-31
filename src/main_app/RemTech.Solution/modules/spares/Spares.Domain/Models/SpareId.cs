using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public sealed record SpareId
{
    public string Value { get; }
    private SpareId(string value) => Value = value;
    
    public static Result<SpareId> Create(string value) =>
        string.IsNullOrWhiteSpace(value) 
            ? Error.Validation("Идентификатор запчасти не может быть пустым.") 
            : Result.Success(new SpareId(value));
}