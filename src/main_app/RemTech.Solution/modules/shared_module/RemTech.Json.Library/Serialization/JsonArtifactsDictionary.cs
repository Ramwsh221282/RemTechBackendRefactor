namespace RemTech.Json.Library.Serialization;

public sealed class JsonArtifactsDictionary
{
    private readonly Dictionary<string, ISerJson> _dictionary = [];

    public void Add(ISerJson artifact)
    {
        string key = artifact.Key();
        if (!_dictionary.TryAdd(key, artifact))
            throw new ArgumentException(
                $"Key: {key} already exists in current json artifact entrance."
            );
    }

    public Queue<ISerJson> ArtifactsQueue() => new(_dictionary.Values);
}
