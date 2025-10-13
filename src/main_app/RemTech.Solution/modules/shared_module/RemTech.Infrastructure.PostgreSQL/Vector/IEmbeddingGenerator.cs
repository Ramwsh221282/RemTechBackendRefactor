namespace RemTech.Infrastructure.PostgreSQL.Vector;

public interface IEmbeddingGenerator : IDisposable
{
    ReadOnlyMemory<float> Generate(string text);
}
