using Microsoft.ML.OnnxRuntime;

namespace RemTech.SharedKernel.NN;

internal readonly ref struct EmbeddingData
{
    private ReadOnlySpan<int> Tokens { get; init; }
    private ReadOnlySpan<int> Indices { get; init; }

    public static EmbeddingData Create(
        List<NamedOnnxValue> tokenizerInputs,
        InferenceSession tokenizerSession
    )
    {
        using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> tokenizerResults =
            tokenizerSession.Run(tokenizerInputs);
        int[] tokens = tokenizerResults[0].AsTensor<int>().ToArray();
        int[] tokenIndices = tokenizerResults[2].AsTensor<int>().ToArray();
        return new EmbeddingData() { Tokens = tokens, Indices = tokenIndices };
    }

    public ReadOnlySpan<int> TokenPairs()
    {
        (int token, int index)[] pairs = new (int token, int index)[Tokens.Length];
        for (int i = 0; i < Tokens.Length; i++)
        {
            pairs[i] = (Tokens[i], Indices[i]);
        }

        Array.Sort(pairs, (a, b) => a.index.CompareTo(b.index));
        int[] result = new int[pairs.Length];
        for (int i = 0; i < pairs.Length; i++)
        {
            result[i] = pairs[i].token;
        }
        return result;
    }
}