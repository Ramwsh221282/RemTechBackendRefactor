using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Shared.Infrastructure.Module.Postgres.Embeddings;

public sealed class OnnxEmbeddingGenerator : IEmbeddingGenerator
{
    private readonly InferenceSession _tokenizerSession;
    private readonly InferenceSession _modelSession;
    private bool _disposed;

    public OnnxEmbeddingGenerator()
    {
        var tokenizerOptions = new SessionOptions();
        tokenizerOptions.RegisterOrtExtensions();
        _tokenizerSession = new InferenceSession(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tokenizer.onnx"),
            tokenizerOptions
        );
        _modelSession = new InferenceSession(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "model.onnx")
        );
    }

    public ReadOnlyMemory<float> Generate(string text)
    {
        DenseTensor<string> stringTensor = new DenseTensor<string>([1]) { [0] = text };

        List<NamedOnnxValue> tokenizerInputs =
        [
            NamedOnnxValue.CreateFromTensor("inputs", stringTensor),
        ];

        EmbeddingData embeddingData = EmbeddingData.Create(tokenizerInputs, _tokenizerSession);
        ReadOnlySpan<int> tokenPairs = embeddingData.TokenPairs();

        DenseTensor<long> inputIdsTensor = new DenseTensor<long>([1, tokenPairs.Length]);
        for (int i = 0; i < tokenPairs.Length; i++)
        {
            inputIdsTensor[0, i] = tokenPairs[i];
        }

        DenseTensor<long> attentionMaskTensor = new DenseTensor<long>([1, tokenPairs.Length]);
        for (int i = 0; i < tokenPairs.Length; i++)
        {
            attentionMaskTensor[0, i] = 1;
        }

        List<NamedOnnxValue> modelInputs =
        [
            NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
            NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor),
        ];

        return GetEmbeddings(modelInputs, _modelSession);
    }

    private static ReadOnlyMemory<float> GetEmbeddings(
        List<NamedOnnxValue> modelInputs,
        InferenceSession modelSession
    )
    {
        using IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? modelResults =
            modelSession.Run(modelInputs);
        float[] sentenceEmbedding = modelResults[1].AsTensor<float>().ToArray();
        return sentenceEmbedding;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _tokenizerSession?.Dispose();
                _modelSession?.Dispose();
            }

            _disposed = true;
        }
    }

    ~OnnxEmbeddingGenerator()
    {
        Dispose(false);
    }
}
