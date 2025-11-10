namespace RemTech.Ner.VehicleParameters;

internal sealed record VehicleNerResult(int WordIdx, string Label, float Confidence);

public sealed record VehicleNerOutput(string Word, string Label, float Confidence);