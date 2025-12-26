namespace RemTech.Ner.VehicleParameters;

internal readonly ref struct InferenceResult
{
    public int NumLabels { get; } 
    public ReadOnlySpan<int> Predictions { get; }
    public Memory<int?> WordIds { get; }
    public ReadOnlySpan<float> Logits { get; }

    public InferenceResult(int numLabels, ReadOnlySpan<int> predictions, Memory<int?> wordIds, ReadOnlySpan<float> logits)
    {
        NumLabels = numLabels;
        Predictions = predictions;
        WordIds = wordIds;
        Logits = logits;
    }
}