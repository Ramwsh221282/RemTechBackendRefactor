using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace RemTech.Vehicles.Module.Database.Embeddings;

internal sealed class OnnxEmbeddingGenerator : IEmbeddingGenerator
{
    private readonly InferenceSession _tokenizerSession;
    private readonly InferenceSession _modelSession;
    private bool _disposed = false;

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

    public float[] Generate(string text)
    {
        DenseTensor<string> stringTensor = new DenseTensor<string>([1]);
        stringTensor[0] = text;

        List<NamedOnnxValue> tokenizerInputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("inputs", stringTensor),
        };

        using IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? tokenizerResults =
            _tokenizerSession.Run(tokenizerInputs);
        List<DisposableNamedOnnxValue> tokenizerResultsList = tokenizerResults.ToList();

        int[] tokens = tokenizerResultsList[0].AsTensor<int>().ToArray();
        int[] tokenIndices = tokenizerResultsList[2].AsTensor<int>().ToArray();

        int[] tokenPairs = tokens
            .Zip(tokenIndices, (t, i) => (token: t, index: i))
            .OrderBy(p => p.index)
            .Select(p => p.token)
            .ToArray();

        DenseTensor<long> inputIdsTensor = new DenseTensor<long>([1, tokenPairs.Length]);
        for (int i = 0; i < tokenPairs.Length; i++)
        {
            inputIdsTensor[0, i] = tokenPairs[i];
        }

        var attentionMaskTensor = new DenseTensor<long>([1, tokenPairs.Length]);
        for (int i = 0; i < tokenPairs.Length; i++)
        {
            attentionMaskTensor[0, i] = 1;
        }

        var modelInputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
            NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor),
        };

        using var modelResults = _modelSession.Run(modelInputs);
        List<DisposableNamedOnnxValue> modelResultsList = modelResults.ToList();
        float[] sentenceEmbedding = modelResultsList[1].AsTensor<float>().ToArray();
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
