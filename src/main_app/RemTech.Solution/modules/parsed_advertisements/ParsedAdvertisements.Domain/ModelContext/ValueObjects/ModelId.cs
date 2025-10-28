using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.ModelContext.ValueObjects;

public readonly record struct ModelId
{
    public Guid Value { get; }

    public ModelId() => Value = Guid.NewGuid();

    private ModelId(Guid value) => Value = value;

    public static Status<ModelId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Error.Validation("Идентификатор модели не может быть пустым.");
        return new ModelId(value);
    }
}
