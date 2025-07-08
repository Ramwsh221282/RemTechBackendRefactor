using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class ParserWithNameNotFoundError : IError
{
    private readonly Error _error;

    public ParserWithNameNotFoundError(NotEmptyString name)
        : this(name.StringValue()) { }

    public ParserWithNameNotFoundError(ParserIdentity identity)
        : this(identity.ReadName()) { }

    public ParserWithNameNotFoundError(IParser parser)
        : this(parser.Identification().ReadName()) { }

    public ParserWithNameNotFoundError(Name name)
        : this(name.NameString()) { }

    private ParserWithNameNotFoundError(string errorString) =>
        _error = Error.NotFound($"Парсер с названием: {errorString} не найден.");

    public Error Read() => _error;

    public static implicit operator Status(ParserWithNameNotFoundError error) => error._error;

    public static implicit operator Status<IParser>(ParserWithNameNotFoundError error) =>
        error._error;
}
