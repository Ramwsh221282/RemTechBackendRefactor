namespace RemTech.Ner.VehicleParameters;

public sealed class VehicleParametersNerOptions
{
    public const int MaxVectorLength = 512;
    public string VocabPath { get; set; } = string.Empty;
    public string Id2LabelPath { get; set; } = string.Empty;
    public string ModelPath { get; set; } = string.Empty;
}