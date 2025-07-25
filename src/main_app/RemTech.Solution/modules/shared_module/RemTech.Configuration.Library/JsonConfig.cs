using Microsoft.Extensions.Configuration;

namespace RemTech.Configuration.Library;

public sealed class JsonConfig : IConfig
{
    private readonly string _filePath;

    public JsonConfig(string filePath)
    {
        _filePath = filePath;
    }

    public IConfigCursor Cursor(string key)
    {
        if (!File.Exists(_filePath))
            throw new ArgumentException("The config file path does not exist.");
        if (!_filePath.EndsWith(".json"))
            throw new ArgumentException("The file path must end with '.json'.");
        return new JsonCursor(new ConfigurationBuilder().AddJsonFile(_filePath).Build().GetSection(key));
    }
}