namespace RemTech.Ner.VehicleParameters;

internal sealed class VehicleNerService : IVehicleNerService
{
    private readonly VehicleNerModelMetadata _metadata;
    private readonly NerModelInference _inference;

    public VehicleNerService(VehicleNerModelMetadata metadata, NerModelInference inference)
    {
        _metadata = metadata;
        _inference = inference;
    }
    
    public IReadOnlyList<VehicleNerOutput> DetectParameters(string input)
    {
        InputWords words = InputsProcessing.GetWords(input);
        WordsTokenization tokenization = words.Tokenize(_metadata);
        IReadOnlyList<VehicleNerOutput> outputs = _inference.GetResults(tokenization, words);
        return outputs;
    }
}