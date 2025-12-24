using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinks.Models;

public sealed record SubscribedParserLinkUrlInfo
{
    private const int MaxNameLength = 255;
    public string Url { get; private init; }
    public string Name { get; private init; }

    private SubscribedParserLinkUrlInfo(string url, string name)
    {
        Url = url;
        Name = name;
    }
    
    public static Result<SubscribedParserLinkUrlInfo> Create(string url, string name)
    {
        if (string.IsNullOrWhiteSpace(url)) 
            return Error.Validation("Ссылка на парсер не может быть пустой.");
        if (string.IsNullOrWhiteSpace(name)) 
            return Error.Validation("Название ссылки на парсер не может быть пустым.");
        if (name.Length > MaxNameLength)
            return Error.Validation($"Название ссылки на парсер не может быть больше {MaxNameLength} символов.");
        return new SubscribedParserLinkUrlInfo(url, name);
    }
}