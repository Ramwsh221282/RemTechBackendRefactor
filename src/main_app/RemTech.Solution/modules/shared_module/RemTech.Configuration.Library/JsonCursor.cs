using Microsoft.Extensions.Configuration;

namespace RemTech.Configuration.Library;

public sealed class JsonCursor : IConfigCursor
{
    private readonly IConfigurationSection _section;

    public JsonCursor(IConfigurationSection section)
    {
        _section = section;
    }
    
    public string GetOption(string key)
    {
        string? option = _section.GetSection(nameof(key)).Value;
        return option ?? throw new ArgumentException($"Option {key} does not exist or not specified.");
    }
}