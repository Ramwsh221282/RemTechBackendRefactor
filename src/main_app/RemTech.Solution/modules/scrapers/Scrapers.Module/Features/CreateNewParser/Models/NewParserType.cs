using Scrapers.Module.Features.CreateNewParser.Exceptions;

namespace Scrapers.Module.Features.CreateNewParser.Models;

internal sealed record NewParserType
{
    private const int AllowedLength = 20;
    public string Type { get; }

    private NewParserType(string type) => Type = type;

    public static NewParserType Create(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ParserTypeEmptyException();
        if (type.Length > AllowedLength)
            throw new ParserTypeExceesLengthException(type, AllowedLength);
        return new NewParserType(type);
    }
}
