using Microsoft.ML.OnnxRuntime;

namespace RemTech.Ner.VehicleParameters;

internal sealed record WordsTokenization(IReadOnlyCollection<NamedOnnxValue> Inputs, Memory<int?> WordIds);