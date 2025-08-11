namespace Shared.Infrastructure.Module.Postgres.Embeddings;

public interface IEmbeddingGenerator : IDisposable
{
    float[] Generate(string text);
}
