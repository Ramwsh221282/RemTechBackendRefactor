using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using RemTech.SharedKernel.Configurations;

namespace RemTech.SharedKernel.NN;

public sealed class EmbeddingsProvider(IOptions<EmbeddingsProviderOptions> options)
{
    private Lazy<InferenceSession> TokenizerSessionLazy { get; } = new(MakeTokenizerSession(options.Value));
    private Lazy<InferenceSession> ModelSessionLazy { get; } = new(MakeModelSession(options.Value));
    
    private InferenceSession Tokenizer => TokenizerSessionLazy.Value;
    private InferenceSession Model => ModelSessionLazy.Value;
    private bool Disposed { get; set; }
    
     public ReadOnlyMemory<float> Generate(string text)
     {
         DenseTensor<string> stringTensor = new DenseTensor<string>(new[] { 1 });
         stringTensor[0] = text;

         NamedOnnxValue[] tokenizerInputs = new[]
         {
             NamedOnnxValue.CreateFromTensor("inputs", stringTensor),
         };

         EmbeddingData embeddingData = EmbeddingData.Create(tokenizerInputs, Tokenizer);

         int len = embeddingData.Length;
         DenseTensor<long> inputIdsTensor = new DenseTensor<long>(new[] { 1, len });
         DenseTensor<long> attentionMaskTensor = new DenseTensor<long>(new[] { 1, len });
         Span<int> sortedTokens = len <= 256 ? stackalloc int[len] : new int[len];
         embeddingData.CopySortedTokensTo(sortedTokens);

         for (int i = 0; i < len; i++)
         {
             inputIdsTensor[0, i] = sortedTokens[i];
             attentionMaskTensor[0, i] = 1;
         }

         var modelInputs = new[]
         {
             NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
             NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor),
         };

         return GetEmbeddings(modelInputs, Model);
     }
     
     private static ReadOnlyMemory<float> GetEmbeddings(
         IReadOnlyList<NamedOnnxValue> modelInputs,
         InferenceSession modelSession)
     {
         using IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? modelResults = modelSession.Run(modelInputs);
         Tensor<float>? tensor = modelResults[1].AsTensor<float>();
         return new ReadOnlyMemory<float>(tensor.ToArray());
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
        if (!Disposed)
        {
            if (disposing)
            {
                TokenizerSessionLazy.Value.Dispose();
                ModelSessionLazy.Value.Dispose();
            }
            Disposed = true;
        }
    }

    ~EmbeddingsProvider()
    {
        Dispose(false);
    }

    private static InferenceSession MakeTokenizerSession(EmbeddingsProviderOptions options)
    {
        options.Validate();
        SessionOptions tokenizerOptions = new();
        tokenizerOptions.RegisterOrtExtensions();
        return new InferenceSession(options.TokenizerPath, tokenizerOptions);
    }

    private static InferenceSession MakeModelSession(EmbeddingsProviderOptions options)
    {
        options.Validate();
        SessionOptions modelOptions = new();
        modelOptions.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
        return new InferenceSession(options.ModelPath, modelOptions);
    }
}