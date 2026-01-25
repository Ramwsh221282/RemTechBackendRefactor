using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public sealed record SparePhoto
{
    public string Value { get; }

    private SparePhoto(string value) => Value = value;

    public static Result<SparePhoto> Create(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? Error.Validation("Фото запчасти не может быть пустым.")
            : Result.Success(new SparePhoto(value));
}
