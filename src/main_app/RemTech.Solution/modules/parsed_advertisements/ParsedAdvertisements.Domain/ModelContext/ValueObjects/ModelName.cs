using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.ModelContext.ValueObjects;

public sealed record ModelName
{
    public const int MaxLength = 100;
    public string Value { get; }

    private ModelName(string value) => Value = value;

    public static Status<ModelName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Название модели техники не может быть пустым.");

        if (value.Length > MaxLength)
            return Error.Validation(
                $"Название модели техники не может превышать длину: {MaxLength} символов."
            );

        return new ModelName(value);
    }
}
