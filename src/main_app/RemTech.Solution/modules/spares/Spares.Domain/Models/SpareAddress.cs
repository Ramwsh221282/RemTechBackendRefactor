using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public sealed record SpareAddress
{
    public string Value { get; }

    private SpareAddress(string value)
    {
        Value = value;
    }

    public static Result<SpareAddress> Create(string value) =>
        string.IsNullOrWhiteSpace(value) ? Error.Validation("Адрес не может быть пустым") : new SpareAddress(value);
}
