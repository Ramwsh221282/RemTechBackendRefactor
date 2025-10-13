using RemTech.Infrastructure.PostgreSQL.Vector;
using Vehicles.Domain.CategoryContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.CategoryContext;

internal static class InfrastructureCategoryExtensions
{
    public static Pgvector.Vector GenerateEmbedding(
        this CategoryName name,
        IEmbeddingGenerator generator
    )
    {
        string nameString = name.Value;
        ReadOnlyMemory<float> vecotrs = generator.Generate(nameString);
        return new Pgvector.Vector(vecotrs);
    }
}