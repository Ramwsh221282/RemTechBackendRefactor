using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public sealed record SpareTextInfo
{
    public string Value { get; }

    private SpareTextInfo(string value) => Value = value;

    public static Result<SpareTextInfo> Create(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? Error.Validation("Описание запчасти не может быть пустым.")
            : Result.Success(new SpareTextInfo(value));
}
