using ParsedAdvertisements.Domain.ModelContext.ValueObjects;

namespace ParsedAdvertisements.Domain.ModelContext;

public sealed class Model
{
    public ModelId Id { get; }
    public ModelName Name { get; }

    public Model(ModelName name, ModelId? id = null)
    {
        Name = name;
        Id = id ?? new ModelId();
    }
}
