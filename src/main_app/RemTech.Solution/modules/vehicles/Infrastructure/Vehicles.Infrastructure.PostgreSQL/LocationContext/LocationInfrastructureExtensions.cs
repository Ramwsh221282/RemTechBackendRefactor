using RemTech.Infrastructure.PostgreSQL.Vector;
using Vehicles.Domain.LocationContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.LocationContext;

internal static class LocationInfrastructureExtensions
{
    public static Pgvector.Vector GenerateVector(
        this LocationAddress address,
        IEmbeddingGenerator generator
    )
    {
        IEnumerable<string> stringParts = address.Parts.Select(p => p.Value);
        string singleString = string.Join(" ", stringParts);
        ReadOnlyMemory<float> vectors = generator.Generate(singleString);
        return new Pgvector.Vector(vectors);
    }
}