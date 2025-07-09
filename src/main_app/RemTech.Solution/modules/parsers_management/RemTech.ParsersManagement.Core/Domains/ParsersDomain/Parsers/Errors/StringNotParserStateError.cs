using RemTech.Core.Shared.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class StringNotParserStateError : IError
{
    private readonly Error _error;

    public StringNotParserStateError(NotEmptyString input) =>
        _error = Error.Validation($"Строка не является состоянием парсера: {input.StringValue()}");

    public Error Read() => _error;

    public static implicit operator Status(StringNotParserStateError error) => new(error._error);
}
