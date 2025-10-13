using System.Text.Json;
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

    public static string LocationToJson(this LocationAddress address)
    {
        return JsonSerializer.Serialize(address, JsonSerializerOptions.Default);
    }

    public static LocationAddress JsonToLocation(this string json)
    {
        return JsonSerializer.Deserialize<LocationAddress>(json, JsonSerializerOptions.Default)!;
    }
}
