namespace RemTech.Vehicles.Module.Database.Embeddings;

public interface IEmbeddingGenerator : IDisposable
{
    float[] Generate(string text);
}
