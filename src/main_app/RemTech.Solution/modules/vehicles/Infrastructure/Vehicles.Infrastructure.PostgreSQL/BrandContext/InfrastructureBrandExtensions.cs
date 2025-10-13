using RemTech.Infrastructure.PostgreSQL.Vector;
using Vehicles.Domain.BrandContext;
using Vehicles.Domain.BrandContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.BrandContext;

internal static class InfrastructureBrandExtensions
{
    public static Pgvector.Vector CreateEmbedding(this Brand brand, IEmbeddingGenerator generator)
    {
        string nameString = brand.Name.Name;
        ReadOnlyMemory<float> embeddings = generator.Generate(nameString);
        return new Pgvector.Vector(embeddings);
    }

    public static Pgvector.Vector CreateEmbedding(
        this BrandName brandName,
        IEmbeddingGenerator generator
    )
    {
        string nameString = brandName.Name;
        ReadOnlyMemory<float> embeddings = generator.Generate(nameString);
        return new Pgvector.Vector(embeddings);
    }
}
