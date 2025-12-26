using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Pgvector;

namespace Shared.Infrastructure.Module.Postgres.Embeddings;

public sealed class OnnxEmbeddingGenerator : IEmbeddingGenerator
{
    public static readonly string DefaultTokenizerPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "tokenizer.onnx"
    );

    public static readonly string DefaultModelPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "model.onnx"
    );

    private readonly Lazy<InferenceSession> _tokenizerSessionLazy;
    private readonly Lazy<InferenceSession> _modelSessionLazy;
    private bool _tokenizerInferenceCreated;
    private bool _modelInferenceCreated;
    private bool _disposed;

    private InferenceSession _tokenizer => _tokenizerSessionLazy.Value;
    private InferenceSession _modelSession => _modelSessionLazy.Value;

    public OnnxEmbeddingGenerator(string tokenizerPath, string modelPath)
    {
        _tokenizerSessionLazy = new Lazy<InferenceSession>(MakeTokenizerSession(tokenizerPath));
        _modelSessionLazy = new Lazy<InferenceSession>(MakeModelSession(modelPath));
    }

    private InferenceSession MakeTokenizerSession(string path)
    {
        if (_tokenizerInferenceCreated)
            return _tokenizerSessionLazy.Value;

        var tokenizerOptions = new SessionOptions();
        tokenizerOptions.RegisterOrtExtensions();
        var session = new InferenceSession(path, tokenizerOptions);
        _tokenizerInferenceCreated = true;
        return session;
    }

    private InferenceSession MakeModelSession(string path)
    {
        if (_modelInferenceCreated)
            return _modelSessionLazy.Value;

        var session = new InferenceSession(path);
        _modelInferenceCreated = true;
        return session;
    }

    public ReadOnlyMemory<float> Generate(string text)
    {
        DenseTensor<string> stringTensor = new DenseTensor<string>([1]) { [0] = text };

        List<NamedOnnxValue> tokenizerInputs =
        [
            NamedOnnxValue.CreateFromTensor("inputs", stringTensor),
        ];

        EmbeddingData embeddingData = EmbeddingData.Create(tokenizerInputs, _tokenizer);
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

    public Vector GenerateVector(string text)
    {
        ReadOnlyMemory<float> vector = Generate(text);
        return new Vector(vector);
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
                _tokenizerSessionLazy.Value.Dispose();
                _tokenizerSessionLazy.Value.Dispose();
            }

            _disposed = true;
        }
    }

    ~OnnxEmbeddingGenerator()
    {
        Dispose(false);
    }
}