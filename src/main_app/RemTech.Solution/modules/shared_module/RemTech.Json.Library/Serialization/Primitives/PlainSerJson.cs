namespace RemTech.Json.Library.Serialization.Primitives;

public sealed class PlainSerJson : ISerJson
{
    private readonly JsonArtifactsDictionary _dictionary;

    public PlainSerJson(PlainSerJson origin, ISerJson artifact)
    {
        origin._dictionary.Add(artifact);
        _dictionary = origin._dictionary;
    }

    public PlainSerJson()
    {
        _dictionary = new JsonArtifactsDictionary();
    }

    public PlainSerJson With(ISerJson artifact)
    {
        return new PlainSerJson(this, artifact);
    }

    public PlainSerJson With(PlainPrimitiveArrayJson array)
    {
        PlainSerJson current = this;
        foreach (ISerJson primitive in array.Read())
            current = new PlainSerJson(current, primitive);
        return current;
    }

    public string Read()
    {
        Queue<string> jsonStrings = [];
        Queue<ISerJson> artifacts = _dictionary.ArtifactsQueue();
        while (artifacts.Count > 0)
            jsonStrings.Enqueue(artifacts.Dequeue().Read());
        return $"{{{string.Join(',', jsonStrings)}}}";
    }

    public string Key() => string.Empty;
}

public sealed class PlainPrimitiveArrayJson
{
    private readonly JsonArtifactsDictionary _dictionary;

    public PlainPrimitiveArrayJson()
    {
        _dictionary = new JsonArtifactsDictionary();
    }

    public PlainPrimitiveArrayJson(PlainPrimitiveArrayJson origin, PrimitiveSerJson json)
    {
        origin._dictionary.Add(json);
        _dictionary = origin._dictionary;
    }

    public PlainPrimitiveArrayJson With(PrimitiveSerJson json)
    {
        return new PlainPrimitiveArrayJson(this, json);
    }

    public IEnumerable<ISerJson> Read()
    {
        return _dictionary.ArtifactsQueue();
    }
}
