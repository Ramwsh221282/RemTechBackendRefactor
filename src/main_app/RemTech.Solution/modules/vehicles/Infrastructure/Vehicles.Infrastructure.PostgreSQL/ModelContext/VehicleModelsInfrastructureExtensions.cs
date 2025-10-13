using RemTech.Infrastructure.PostgreSQL.Vector;
using Vehicles.Domain.ModelContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.ModelContext;

internal static class VehicleModelsInfrastructureExtensions
{
    public static Pgvector.Vector Generate(
        this VehicleModelName name,
        IEmbeddingGenerator generator
    )
    {
        string nameString = name.Value;
        ReadOnlyMemory<float> vectors = generator.Generate(nameString);
        return new Pgvector.Vector(vectors);
    }
}