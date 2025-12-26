using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace RemTech.Ner.VehicleParameters;

internal sealed class VehicleNerModelMetadata
{
    public IReadOnlyDictionary<string, int> Vocab { get; }
    public IReadOnlyDictionary<int, string> Id2Label { get; }

    public VehicleNerModelMetadata(IOptions<VehicleParametersNerOptions> options)
    {
        Vocab = LoadVocab(options.Value);
        Id2Label = LoadId2Label(options.Value);
    }

    private static IReadOnlyDictionary<string, int> LoadVocab(in VehicleParametersNerOptions options)
    {
        return File.ReadAllLines(options.VocabPath)
            .Select((line, idx) => new { Token = line.Trim(), Id = idx })
            .ToDictionary(x => x.Token, x => x.Id);
    }

    private static IReadOnlyDictionary<int, string> LoadId2Label(in VehicleParametersNerOptions options)
    {
        string rawText = File.ReadAllText(options.Id2LabelPath); 
        Dictionary<string, string> id2labelRaw = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawText)!;
        return id2labelRaw.ToDictionary(kvp => int.Parse(kvp.Key), kvp => kvp.Value);
    }
}