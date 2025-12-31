using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public sealed record SpareType
{
    public string Value { get; }

    private SpareType(string value)
    {
        Value = value;
    }

    public static Result<SpareType> Create(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Error.Validation("Тип запчасти не может быть пустым")
            : new SpareType(value);
    }
}