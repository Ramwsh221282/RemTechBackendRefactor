namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;

public sealed class ParserLinkPrint
{
    private readonly string _text;

    public ParserLinkPrint(IParserLink link)
    {
        _text = $"""
            Информация о ссылке:
            ID: {(Guid)link.Identification().ReadId()},
            Название: {(string)link.Identification().ReadName().NameString()},
            URL: {(string)link.ReadUrl().Read()},
            Активна: {link.Activity().Read()}
            """;
    }

    public string Read() => _text;
}
