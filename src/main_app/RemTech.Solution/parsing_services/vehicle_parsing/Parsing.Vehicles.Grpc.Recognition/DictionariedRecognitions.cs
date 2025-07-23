namespace Parsing.Vehicles.Grpc.Recognition;

public sealed class DictionariedRecognitions
{
    private readonly Dictionary<string, Characteristic> _recognized = [];
    private readonly List<ICharacteristicRecognition> _recognitions = [];
    
    public DictionariedRecognitions With(ICharacteristicRecognition recognition)
    {
        _recognitions.Add(recognition);
        return this;
    }

    public async Task<DictionariedRecognitions> Processed(string text)
    {
        Characteristic[] results = await Task.WhenAll(_recognitions.Select(r => r.Recognize(text)));
        foreach (var result in results)
        {
            if (!result)
                continue;
            if (_recognized.ContainsKey(result.ReadName()))
                continue;
            _recognized.Add(result.ReadName(), result);
        }

        return this;
    }

    public Characteristic ByName(string name)
    {
        return _recognized.TryGetValue(name, out Characteristic? value) ? value : new Characteristic();
    }

    public IEnumerable<Characteristic> All() => _recognized.Values;
}