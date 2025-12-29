namespace RemTech.SharedKernel.NN;

public sealed class EmbeddingsProvider
{
    public ReadOnlyMemory<float> Generate(string text)
    {
        float[] embeddings = [];
        return new ReadOnlyMemory<float>(embeddings);
    }
}