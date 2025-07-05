namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;

public sealed class ParserLinkPrint
{
    private readonly string _text;

    public ParserLinkPrint(IParserLink link)
    {
        _text = $"""
            Информация о ссылке:
            ID: {link.Identification().ReadId().GuidValue()},
            Название: {link.Identification().ReadName().NameString()},
            URL: {link.ReadUrl().Read().StringValue()},
            Активна: {link.Activity().Read()}
            """;
    }

    public string Read() => _text;
}
