using Pgvector;

namespace Shared.Infrastructure.Module.Postgres.Embeddings;

public interface IEmbeddingGenerator : IDisposable
{
    ReadOnlyMemory<float> Generate(string text);
    Vector GenerateVector(string text);
}