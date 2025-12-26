using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace RemTech.Ner.VehicleParameters;

internal sealed class NerModelInference : IDisposable
{
    private readonly Lazy<InferenceSession> _session;
    private readonly VehicleNerModelMetadata _metadata;
    public InferenceSession Session => _session.Value;
    
    public NerModelInference(IOptions<VehicleParametersNerOptions> options, VehicleNerModelMetadata metadata)
    {
        _session = new Lazy<InferenceSession>(() => new InferenceSession(options.Value.ModelPath));
        _metadata = metadata;
    }

    public void Dispose()
    {
        if (_session.IsValueCreated)
            _session.Value.Dispose();
    }

    public IReadOnlyList<VehicleNerOutput> GetResults(WordsTokenization tokenization, InputWords words)
    {
        InferenceResult inference = Run(tokenization);
        Dictionary<int, VehicleNerResult> nerResults = new();
        for (int i = 0; i < inference.WordIds.Length; i++)
        {
            int? wordIdx = inference.WordIds.Span[i];
            if (wordIdx is null or < 0)
                continue;
            
            if (i > 0 && inference.WordIds.Span[i - 1] == wordIdx)
                continue;
            
            int predictionId = inference.Predictions[i];
            if (!_metadata.Id2Label.TryGetValue(predictionId, out string label) || label == "O")
                continue;

            int offset = i * inference.NumLabels;
            ReadOnlySpan<float> logitsSlice = inference.Logits.Slice(offset, inference.NumLabels);
            
            float maxLogit = GetMaxLogit(ref logitsSlice);
            float sumExp = GetSumExp(ref logitsSlice, ref maxLogit);
            float confidence = GetConfidence(ref logitsSlice, ref maxLogit, ref sumExp, ref predictionId);
            
            if (!nerResults.TryGetValue(wordIdx.Value, out VehicleNerResult? existing) || confidence > existing.Confidence)
                nerResults[wordIdx.Value] = new VehicleNerResult(wordIdx.Value, label, confidence);
        }

        List<VehicleNerOutput> results = [];
        foreach (KeyValuePair<int, VehicleNerResult> kvp in nerResults)
        {
            string label = kvp.Value.Label;
            string word = words.Words[kvp.Value.WordIdx];
            float confidence = kvp.Value.Confidence;
            VehicleNerOutput output = new(word, label, confidence);
            results.Add(output);
        }

        return results;
    }

    private static float GetMaxLogit(ref ReadOnlySpan<float> logitsSlice)
    {
        float maxLogit = logitsSlice[0];
        for (int j = 1; j < logitsSlice.Length; j++)
            if (logitsSlice[j] > maxLogit) maxLogit = logitsSlice[j];
        return maxLogit;
    }

    private static float GetSumExp(ref ReadOnlySpan<float> logitsSlice, ref float maxLogit)
    {
        float sumExp = 0.0f;
        for (int j = 0; j < logitsSlice.Length; j++)
            sumExp += MathF.Exp(logitsSlice[j] - maxLogit);
        return sumExp;
    }

    private static float GetConfidence(ref ReadOnlySpan<float> logitsSlice, ref float maxLogit, ref float sumExp, ref int predictionId)
    {
        return MathF.Exp(logitsSlice[predictionId] - maxLogit) / sumExp;
    }
    
    private InferenceResult Run(WordsTokenization tokenization)
    {
        const int MaxLength = VehicleParametersNerOptions.MaxVectorLength;
        using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> outputs = Session.Run(tokenization.Inputs);
        DenseTensor<float> logitsTensor = (DenseTensor<float>)outputs[0].Value;
        int numLabels = logitsTensor.Dimensions[2];
        Span<int> predictions = new(new int[MaxLength]);
        for (int i = 0; i < predictions.Length; i++)
        {
            int bestIdx = 0;
            float bestLogit = logitsTensor.Buffer.Span[i * numLabels];

            for (int j = 1; j < numLabels; j++)
            {
                if (!(logitsTensor.Buffer.Span[i * numLabels + j] > bestLogit)) continue;
                
                bestLogit = logitsTensor.Buffer.Span[i * numLabels + j];
                bestIdx = j;
            }
            
            predictions[i] = bestIdx;
        }

        return new InferenceResult(numLabels, predictions, tokenization.WordIds, logitsTensor.Buffer.Span);
    }
}