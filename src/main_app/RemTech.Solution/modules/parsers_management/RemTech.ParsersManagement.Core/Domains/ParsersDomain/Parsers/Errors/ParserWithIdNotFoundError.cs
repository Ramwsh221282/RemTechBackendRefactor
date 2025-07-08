using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class ParserWithIdNotFoundError : IError
{
    private readonly Error _error;

    public ParserWithIdNotFoundError(NotEmptyGuid id)
        : this((Guid)id) { }

    public ParserWithIdNotFoundError(ParserIdentity identity)
        : this(identity.ReadId()) { }

    private ParserWithIdNotFoundError(Guid id)
    {
        string message = $"Парсер с ID: {id} не найден.";
        _error = (message, ErrorCodes.Conflict);
    }

    public static implicit operator Status(ParserWithIdNotFoundError error) => error._error;

    public static implicit operator Status<IParser>(ParserWithIdNotFoundError error) =>
        error._error;

    public Error Read() => _error;
}
